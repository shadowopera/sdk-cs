#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Provides core utility functions for Atlas loading and configuration management.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Loads an Atlas synchronously from the specified manifest file and root directory.
        /// </summary>
        /// <remarks>
        /// <para>This method performs the following steps:</para>
        /// <list type="number">
        /// <item><description>Reads and parses atlas.json</description></item>
        /// <item><description>Applies any registered modifiers to the atlas data</description></item>
        /// <item><description>Loads each configuration item by reading files, deserializing, and merging overrides</description></item>
        /// <item><description>Calls BindRefs to resolve cross-table references</description></item>
        /// <item><description>Calls OnLoaded on the Atlas for post-load initialization</description></item>
        /// </list>
        /// <para>If any step fails, an ArchmageException is raised and loading is aborted.
        /// Exceptions can be thrown from IAtlas.OnLoaded() to abort loading.</para>
        /// <para>Atlas items are parsed on thread pool threads, unless
        /// <see cref="AtlasOptionExtensions.WithMainThreadParsing"/> is set. So <see cref="IApplyKeys.ApplyKeys"/>,
        /// the logger and <paramref name="progress"/> may be called on thread pool threads. The atlas modifier,
        /// BindRefs and OnLoaded run on the calling thread.</para>
        /// </remarks>
        /// <param name="atlasFile">Path to atlas.json containing mapping definitions.</param>
        /// <param name="cfgRoot">Root directory where configuration JSON files are located.</param>
        /// <param name="atlas">The Atlas implementation to populate with loaded items.</param>
        /// <param name="options">Optional loading configuration. If null, default options are used.</param>
        /// <param name="progress">Optional callback for receiving progress reports.</param>
        /// <exception cref="ArchmageException">Thrown if loading fails at any stage.</exception>
        public static void LoadAtlas(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions? options = null,
            IProgress<AtlasLoadEvent>? progress = null)
        {
            options ??= new AtlasOptions();
            LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, false, false, progress).GetAwaiter().GetResult();
            atlas.OnLoaded();
        }

        /// <summary>
        /// Loads an Atlas asynchronously with progress reporting and cancellation support.
        /// </summary>
        /// <remarks>
        /// <para>This method performs the same steps as <see cref="LoadAtlas"/>.</para>
        /// <para>Unless <paramref name="workerThreadLoading"/> is true, everything runs on the same threads as in
        /// <see cref="LoadAtlas"/>. If the calling thread has no <see cref="SynchronizationContext"/>, the work that
        /// LoadAtlas does on the calling thread may run on thread pool threads instead.</para>
        /// <para>Do not block on the returned task on a thread that has a synchronization context, such as a UI
        /// thread or the main thread of a game engine; it deadlocks. Use <see cref="LoadAtlas"/> for synchronous
        /// loading.</para>
        /// </remarks>
        /// <param name="atlasFile">Path to atlas.json containing mapping definitions.</param>
        /// <param name="cfgRoot">Root directory where configuration JSON files are located.</param>
        /// <param name="atlas">The Atlas implementation to populate with loaded items.</param>
        /// <param name="options">Optional loading configuration. If null, default options are used.</param>
        /// <param name="workerThreadLoading">True to run the whole load except <see cref="IAtlas.OnLoaded"/> on thread
        /// pool threads. The atlas modifier, <see cref="IAtlas.BindRefs"/>, the logger and
        /// <see cref="IProgress{T}.Report"/> must then not call APIs that work only on the main thread, such as the
        /// Godot scene tree. A <see cref="Progress{T}"/> created on the main thread still runs its handler there.</param>
        /// <param name="progress">Optional callback for receiving progress reports.</param>
        /// <param name="cancellationToken">Token to request cancellation of the loading operation.</param>
        /// <returns>A Task representing the asynchronous loading operation.</returns>
        /// <exception cref="ArchmageException">Thrown if loading fails at any stage.</exception>
        /// <exception cref="OperationCanceledException">Thrown if cancellation is requested.</exception>
        public static async Task LoadAtlasAsync(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions? options = null,
            bool workerThreadLoading = false,
            IProgress<AtlasLoadEvent>? progress = null,
            CancellationToken cancellationToken = default)
        {
            options ??= new AtlasOptions();
            if (workerThreadLoading)
            {
                if (AnyMainThreadOnly(options))
                    throw new ArchmageException("workerThreadLoading cannot be used when MainThreadOnly is true for an IFS.");
                if (options.MainThreadParsing)
                    throw new ArchmageException("workerThreadLoading cannot be used with MainThreadParsing.");
                await Task.Run(() => LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, true, true, progress, cancellationToken),
                    cancellationToken);
            }
            else
            {
                await LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, true, false, progress, cancellationToken);
            }

            // Runs on the caller's synchronization context, if any.
            atlas.OnLoaded();
        }

        static async Task LoadAtlasImpl(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions options,
            bool isAsync,
            bool workerThreadLoading,
            IProgress<AtlasLoadEvent>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var jsonSettings = CreateJsonLoadSettings(options.JsonSettings);

            // Verify override directories exist
            foreach (var overrideConfig in options.OverrideConfigs)
            {
                if (overrideConfig.FS is null)
                {
                    if (!options.FS.DirectoryExists(overrideConfig.RootPath!))
                        throw new ArchmageException($"Invalid override root directory \"{overrideConfig.RootPath}\".");
                }
                else if (overrideConfig.RootPath is not null)
                {
                    if (!overrideConfig.FS.DirectoryExists(overrideConfig.RootPath))
                        throw new ArchmageException($"Invalid override root directory \"{overrideConfig.RootPath}\".");
                }
            }

            // Mode C when an IFS works only on the main thread: items are read on the caller's thread or context.
            // Mode W otherwise: items are read with ReadAllBytes, even when loading asynchronously.
            var modeC = AnyMainThreadOnly(options);
            Func<IFS, string, CancellationToken, Task<byte[]>> readFile = isAsync && modeC
                ? (fs, path, ct) => fs.ReadAllBytesAsync(path, ct)
                : (fs, path, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    return Task.FromResult(fs.ReadAllBytes(path));
                };

            cancellationToken.ThrowIfCancellationRequested();

            // Read and parse atlas.json
            // Why ReadAllBytes in Mode W when MainThreadParsing or workerThreadLoading is set, even when loading
            // asynchronously? To avoid the switch to a thread pool thread that ReadAllBytesAsync may make: an IFS that
            // can be called on any thread may implement it with Task.Run, as GodotFileAccessFS does. MainThreadParsing
            // must not use the thread pool, and workerThreadLoading already runs on it.
            byte[] atlasData;
            if (!isAsync || (!modeC && (options.MainThreadParsing || workerThreadLoading)))
                atlasData = options.FS.ReadAllBytes(atlasFile);
            else
                atlasData = await options.FS.ReadAllBytesAsync(atlasFile, cancellationToken);
            AtlasJson? atlasJson;
            try
            {
                atlasJson = JsonConvert.DeserializeObject<AtlasJson>(Encoding.UTF8.GetString(atlasData), jsonSettings);
            }
            catch (JsonException ex)
            {
                throw new ArchmageException($"Invalid \"{atlasFile}\".", ex);
            }

            if (atlasJson is null)
            {
                throw new ArchmageException($"Invalid \"{atlasFile}\".");
            }

            // Apply modifier
            options.AtlasModifier?.Invoke(atlasJson);

            // Set version info
            atlas.SetDataVersion(atlasJson.Version);

            // Get all items
            var items = atlas.AtlasItems();

            // Validate whitelist/blacklist keys exist
            if (options.Whitelist is not null)
            {
                foreach (var v in options.Whitelist)
                {
                    if (!items.ContainsKey(v))
                        throw new ArchmageException($"Atlas whitelist: unknown item \"{v}\".");
                }
            }
            if (options.Blacklist is not null)
            {
                foreach (var v in options.Blacklist)
                {
                    if (!items.ContainsKey(v))
                        throw new ArchmageException($"Atlas blacklist: unknown item \"{v}\".");
                }
            }

            // Validate variant selections
            foreach (var v in options.Variants.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
            {
                if (!items.ContainsKey(v))
                    throw new ArchmageException($"Atlas variant: unknown item \"{v}\".");
                if (string.IsNullOrEmpty(options.Variants[v]))
                    throw new ArchmageException($"Atlas variant: empty variant for item \"{v}\".");
            }

            // Sort by key (case-insensitive) and filter
            var sortedKeys = items.Keys
                .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var filtered = new List<KeyValuePair<string, AtlasItem>>();
            foreach (var k in sortedKeys)
            {
                var (cause, skip) = ShouldSkip(k, options);
                if (skip)
                {
                    options.Logger.Info($"<archmage> Skipping atlas item: {k}. cause: {cause}");
                    continue;
                }
                filtered.Add(new KeyValuePair<string, AtlasItem>(k, items[k]));
            }

            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new AtlasLoadEvent("", AtlasLoadStage.ItemsQueued, total: filtered.Count));

            // Load atlas items. Where each item is read and parsed:
            //   Mode C: read here, parsed on a thread pool thread.
            //   Mode W: read and parsed on a thread pool thread.
            //   Either mode with MainThreadParsing: read and parsed here.
            // Why no ConfigureAwait(false) on the awaits before a read? In Mode C, IFS implementations need the caller's
            // context: the Unity file systems work only on the main thread. Leaving the context would make each of
            // their reads switch back to the main thread, which costs about one frame per file.
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var tasks = new List<Task>(filtered.Count);
                var running = new List<Task>();
                Task? firstFailure = null;

                // Removes finished items and, on the first failure, cancels the rest.
                // Why cancel here, not in the failing item's catch? That catch runs on a thread pool thread.
                // Cancel() runs the token's callbacks on the thread that calls it, and an IFS implementation may
                // register one that must run on the main thread.
                void Reap()
                {
                    for (var i = running.Count - 1; i >= 0; i--)
                    {
                        var t = running[i];
                        if (!t.IsCompleted)
                            continue;
                        running.RemoveAt(i);
                        if (t.IsFaulted && firstFailure is null)
                        {
                            firstFailure = t;
                            // ReSharper disable once AccessToDisposedClosure
                            cts.Cancel();
                        }
                    }
                }

                // Do not replace this window with SemaphoreSlim.WaitAsync. On Unity WebGL, a waiter that starts
                // waiting after the semaphore is exhausted is never woken.
                async Task WaitAnyAsync()
                {
                    if (isAsync)
                        await Task.WhenAny(running);
                    else
                        // Must not await: sync mode blocks the caller, which may own a SynchronizationContext
                        // that the continuation would be posted to, causing a deadlock.
                        Task.WaitAny(running.ToArray());
                    Reap();
                }

                foreach (var kvp in filtered)
                {
                    Reap();
                    if (running.Count >= options.MaxConcurrency)
                        await WaitAnyAsync();
                    if (cts.IsCancellationRequested)
                        break;

                    var task = ProcessItemAsync(kvp.Key, kvp.Value, cts.Token);
                    tasks.Add(task);
                    running.Add(task);
                }

                // Wait for everything, including items still running after a failure.
                while (running.Count > 0)
                    await WaitAnyAsync();

                if (firstFailure is not null)
                    ExceptionDispatchInfo.Capture(firstFailure.Exception!.InnerException!).Throw();
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var t in tasks)
                    t.GetAwaiter().GetResult();
            }

            async Task ProcessItemAsync(string key, AtlasItem atlasItem, CancellationToken ct)
            {
                try
                {
                    if (!modeC && !options.MainThreadParsing)
                    {
                        await Task.Run(async () =>
                        {
                            // readFile is synchronous in Mode W, so this await completes without waiting.
                            var loadingItem = await ReadItemAsync(key, atlasItem, atlasJson, atlasFile, cfgRoot, options, readFile, progress, ct);
                            UnmarshalItem(loadingItem, options, jsonSettings, progress, ct);
                        }, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        var loadingItem = await ReadItemAsync(key, atlasItem, atlasJson, atlasFile, cfgRoot, options, readFile, progress, ct);
                        if (options.MainThreadParsing)
                            UnmarshalItem(loadingItem, options, jsonSettings, progress, ct);
                        else
                            // Nothing after this point touches IFS, so there is no need to resume on the caller's context.
                            await Task.Run(() => UnmarshalItem(loadingItem, options, jsonSettings, progress, ct), ct).ConfigureAwait(false);
                    }
                }
                // An OperationCanceledException that ct did not cause, such as an IFS timeout, is a failure.
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    var msg = $"Failed to load atlas item: \"{key}\". atlasFile: {atlasFile}, cfgRoot: {cfgRoot}";
                    throw new ArchmageException(msg, ex);
                }
            }

            // Bind references
            atlas.BindRefs();

            options.Logger.Info($"<archmage> Loaded {filtered.Count} atlas items in {stopwatch.ElapsedMilliseconds}ms");
        }

        internal static JsonSerializerSettings CloneJsonSettings(JsonSerializerSettings? original)
        {
            var settings = new JsonSerializerSettings();
            if (original is not null)
            {
                foreach (var prop in typeof(JsonSerializerSettings).GetProperties())
                {
                    if (prop.CanWrite)
                        prop.SetValue(settings, prop.GetValue(original));
                }

                // The loop copies the reference to the caller's list; copy the list so that adding converters
                // leaves the caller's settings unchanged.
                settings.Converters = new List<JsonConverter>(original.Converters);
            }

            return settings;
        }

        static JsonSerializerSettings CreateJsonLoadSettings(JsonSerializerSettings? baseSettings)
        {
            var settings = CloneJsonSettings(baseSettings);
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new DateTimeOffsetJsonConverter());
            return settings;
        }

        static bool AnyMainThreadOnly(AtlasOptions options)
        {
            return options.FS.MainThreadOnly || options.OverrideConfigs.Any(c => c.FS is not null && c.FS.MainThreadOnly);
        }

        static (string cause, bool skip) ShouldSkip(string key, AtlasOptions options)
        {
            if (options.Whitelist is not null && options.Whitelist.Count > 0)
                return ("whitelist", !options.Whitelist.Contains(key));

            if (options.Blacklist is not null && options.Blacklist.Count > 0 && options.Blacklist.Contains(key))
                return ("blacklist", true);

            return ("", false);
        }

        /// <summary>
        /// Resolves the files of an atlas item. Returns null for an unsupported mapping.
        /// </summary>
        static List<string>? ResolveFiles(string key, AtlasItem atlasItem, AtlasJson atlasJson, AtlasOptions options,
            out string keyPath, out string? variant)
        {
            variant = null;
            switch (atlasItem.Mapping)
            {
                case AtlasConstants.MappingUnique:
                    keyPath = $"$.unique['{key}']";
                    return atlasJson.Unique.TryGetValue(key, out var uf) ? new List<string> { uf } : new List<string>();
                case AtlasConstants.MappingVariant:
                    variant = options.Variants.GetValueOrDefault(key, AtlasConstants.VariantMappingDefaultKey);
                    keyPath = $"$.variant['{key}']['{variant}']";
                    var sf = atlasJson.PickFromVariant(key, variant);
                    return sf is not null ? new List<string> { sf } : new List<string>();
                case AtlasConstants.MappingMany:
                    keyPath = $"$.many['{key}']";
                    return atlasJson.Many.TryGetValue(key, out var mf) ? mf : new List<string>();
                default:
                    keyPath = "";
                    return null;
            }
        }

        static string OverridePath(OverrideConfig overrideCfg, string file)
        {
            return overrideCfg.RootPath is not null ? Path.Combine(overrideCfg.RootPath, file) : file;
        }

        /// <summary>
        /// Files of an atlas item that have been read and are waiting to be parsed.
        /// </summary>
        sealed class LoadingItem
        {
            public LoadingItem(AtlasItem atlasItem, List<string> filePaths, byte[][] fileBlobs,
                List<(string Path, byte[] Data)> overrides, Stopwatch stopwatch)
            {
                Item = atlasItem;
                FilePaths = filePaths;
                FileBlobs = fileBlobs;
                Overrides = overrides;
                Stopwatch = stopwatch;
            }

            public AtlasItem Item { get; }
            public List<string> FilePaths { get; }
            public byte[][] FileBlobs { get; }
            public List<(string Path, byte[] Data)> Overrides { get; }
            public Stopwatch Stopwatch { get; }
        }

        /// <summary>
        /// Reads the primary files and override files of an atlas item, concurrently when readFile is asynchronous.
        /// Runs on a thread pool thread in Mode W without MainThreadParsing, and on the caller's thread or context
        /// otherwise.
        /// </summary>
        static async Task<LoadingItem> ReadItemAsync(
            string key,
            AtlasItem atlasItem,
            AtlasJson atlasJson,
            string atlasFile,
            string cfgRoot,
            AtlasOptions options,
            Func<IFS, string, CancellationToken, Task<byte[]>> readFile,
            IProgress<AtlasLoadEvent>? progress,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            atlasItem.Key = key;

            var files = ResolveFiles(key, atlasItem, atlasJson, options, out var keyPath, out var variant)
                ?? throw new Exception($"Unsupported mapping: {atlasItem.Mapping}.");
            if (variant is not null)
                atlasItem.Variant = variant;

            if (files.Count == 0)
            {
                throw new Exception($"Could not find {keyPath} in {atlasFile}.");
            }

            var stopwatch = Stopwatch.StartNew();

            // Report: StartProcessing
            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartProcessing, elapsed: stopwatch.Elapsed));

            var filePaths = new List<string>(files.Count);
            var fileReads = new List<Task<byte[]>>(files.Count);
            var overridePaths = new List<string>();
            var overrideReads = new List<Task<byte[]?>>();
            foreach (var f in files)
            {
                var filePath = Path.Combine(cfgRoot, f);

                // Report: StartReading
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartReading, filePath, stopwatch.Elapsed));

                filePaths.Add(filePath);
                fileReads.Add(Read(options.FS, filePath));

                foreach (var overrideCfg in options.OverrideConfigs)
                {
                    var fs = overrideCfg.FS ?? options.FS;
                    var ovrPath = OverridePath(overrideCfg, f);

                    if (!fs.FileExists(ovrPath))
                        continue;

                    // Report: StartReadingOverride
                    progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartReadingOverride, ovrPath, stopwatch.Elapsed));

                    overridePaths.Add(ovrPath);
                    overrideReads.Add(ReadOptional(fs, ovrPath));
                }
            }

            // Wait for every read, even after one fails.
            await Task.WhenAll(fileReads.Cast<Task>().Concat(overrideReads));

            var overrides = new List<(string Path, byte[] Data)>(overrideReads.Count);
            for (var i = 0; i < overrideReads.Count; i++)
            {
                var data = overrideReads[i].Result;
                if (data is not null)
                    overrides.Add((overridePaths[i], data));
            }

            return new LoadingItem(atlasItem, filePaths, fileReads.Select(t => t.Result).ToArray(), overrides, stopwatch);

            // readFile can throw synchronously before returning a Task. This async wrapper
            // captures that exception and returns a faulted Task instead, so callers can handle
            // all failures uniformly with await/catch.
            async Task<byte[]> Read(IFS fs, string path)
            {
                return await readFile(fs, path, ct);
            }

            async Task<byte[]?> ReadOptional(IFS fs, string path)
            {
                try
                {
                    return await readFile(fs, path, ct);
                }
                catch (FileNotFoundException)
                {
                    // FileExists may report true for a missing file.
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Deserializes the files of an atlas item and applies its overrides.
        /// </summary>
        static void UnmarshalItem(LoadingItem loadingItem, AtlasOptions options, JsonSerializerSettings jsonSettings, IProgress<AtlasLoadEvent>? progress, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var atlasItem = loadingItem.Item;
            var key = atlasItem.Key;
            var stopwatch = loadingItem.Stopwatch;

            for (var i = 0; i < loadingItem.FilePaths.Count; i++)
            {
                // Report: StartParsing
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartParsing, loadingItem.FilePaths[i], stopwatch.Elapsed));

                var json = Encoding.UTF8.GetString(loadingItem.FileBlobs[i]);
                MergeJson(atlasItem.Cfg!, json, jsonSettings);
            }

            // Apply all overrides
            foreach (var (path, data) in loadingItem.Overrides)
            {
                // Report: ApplyingOverride
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.ApplyingOverride, path, stopwatch.Elapsed));

                var overrideJson = Encoding.UTF8.GetString(data);
                try
                {
                    MergeJson(atlasItem.Cfg!, overrideJson, jsonSettings);
                }
                catch (JsonException ex)
                {
                    throw new Exception($"Failed to apply override {path}.", ex);
                }
            }

            // Report: Completed
            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.Completed, elapsed: stopwatch.Elapsed));
            stopwatch.Stop();

            // Call ApplyKeys if implemented
            if (atlasItem.Cfg is IApplyKeys applyKeys)
            {
                applyKeys.ApplyKeys();
            }

            // Build log supplement
            var supplement = loadingItem.Overrides.Count switch
            {
                0 => "",
                1 => " with 1 override",
                _ => $" with {loadingItem.Overrides.Count} overrides"
            };

            var mapping = atlasItem.Mapping == AtlasConstants.MappingVariant
                ? $"mapping={atlasItem.Mapping}, variant={atlasItem.Variant}"
                : $"mapping={atlasItem.Mapping}";
            var paths = string.Join(", ", loadingItem.FilePaths);
            options.Logger.Info($"<archmage> Loaded atlas item \"{key}\" ({mapping}) from {paths}{supplement} in {stopwatch.ElapsedMilliseconds}ms");
            atlasItem.Ready = true;
        }

        static readonly JsonMergeSettings _mergeSettings = new()
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        /// <summary>
        /// Merges JSON values into an existing object using the following rules:
        /// <list type="bullet">
        ///   <item><c>null</c> → resets the target field to its default value or raise an error</item>
        ///   <item>JSON object → recursively merges: only fields present in the input are updated, others remain unchanged</item>
        ///   <item>Any other value → overwrites the field</item>
        /// </list>
        /// Like the Go SDK, which follows Go's <c>json.Unmarshal</c> exactly, <c>null</c> is a regular value,
        /// not a deletion marker. A <c>null</c> dictionary entry keeps its key and holds the default value.
        /// Replacing a whole dictionary or removing individual entries is left to the application layer.
        /// </summary>
        /// <exception cref="ArchmageException">Thrown if JSON is invalid or merge fails.</exception>
        static void MergeJson(object target, string json, JsonSerializerSettings? settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            var targetToken = JToken.FromObject(target, jsonSerializer);

            using var stringReader = new StringReader(json);
            using var jsonReader = new JsonTextReader(stringReader)
            {
                DateParseHandling = jsonSerializer.DateParseHandling,
                FloatParseHandling = jsonSerializer.FloatParseHandling,
                DateTimeZoneHandling = jsonSerializer.DateTimeZoneHandling,
                Culture = jsonSerializer.Culture
            };
            var patch = JToken.Load(jsonReader);

            if (targetToken is JArray && patch is JArray && target is System.Collections.IList listTarget)
            {
                listTarget.Clear();
                jsonSerializer.Populate(patch.CreateReader(), listTarget);
                return;
            }

            if (targetToken is JContainer targetContainer && patch is JContainer patchContainer)
            {
                targetContainer.Merge(patchContainer, _mergeSettings);
            }

            jsonSerializer.ObjectCreationHandling = ObjectCreationHandling.Replace;
            using var reader = targetToken.CreateReader();
            jsonSerializer.Populate(reader, target);
        }
    }

    /// <summary>
    /// Called after deserialization/overrides, before marking Ready.
    /// </summary>
    public interface IApplyKeys
    {
        void ApplyKeys();
    }
}
