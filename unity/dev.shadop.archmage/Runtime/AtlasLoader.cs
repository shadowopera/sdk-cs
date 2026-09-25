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
        /// <item><description>Applies any registered modifier callbacks to the atlas data</description></item>
        /// <item><description>Loads each configuration item by reading files, deserializing, and merging overrides</description></item>
        /// <item><description>Calls BindRefs to resolve cross-table references</description></item>
        /// <item><description>Calls OnLoaded on the Atlas for post-load initialization</description></item>
        /// </list>
        /// <para>If any step fails, an ArchmageException is raised and loading is aborted.
        /// Alternatively, exceptions can be thrown from IAtlas.OnLoaded() to abort loading.</para>
        /// <para>Files are read on the calling thread. Items are deserialized in parallel on the thread pool,
        /// so <see cref="IApplyKeys.ApplyKeys"/>, the logger and <paramref name="progress"/> may be called
        /// from thread pool threads. The atlas modifier, BindRefs and OnLoaded run on the calling thread.</para>
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
            LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, false, progress).GetAwaiter().GetResult();
            atlas.OnLoaded();
        }

        /// <summary>
        /// Loads an Atlas asynchronously with progress reporting and cancellation support.
        /// </summary>
        /// <remarks>
        /// <para>This method performs the same operations as LoadAtlas but asynchronously, providing non-blocking I/O.
        /// Progress events are reported via the IProgress interface to allow UI updates and status tracking.
        /// Loading can be canceled via the CancellationToken.</para>
        /// <para>All <see cref="IFS"/> calls are made on the caller's synchronization context (the Unity main thread
        /// when called from it), so file systems can use main-thread-only APIs. Items are deserialized in parallel
        /// on the thread pool, so <see cref="IApplyKeys.ApplyKeys"/>, the logger and <paramref name="progress"/>
        /// may be called from thread pool threads (<see cref="Progress{T}"/> posts back to its own context).
        /// The atlas modifier, BindRefs and OnLoaded run on the caller's context.</para>
        /// <para>Do not block on the returned task on a thread that has a synchronization context, such as the
        /// Unity main thread; it deadlocks. Use <see cref="LoadAtlas"/> for synchronous loading.</para>
        /// </remarks>
        /// <param name="atlasFile">Path to atlas.json containing mapping definitions.</param>
        /// <param name="cfgRoot">Root directory where configuration JSON files are located.</param>
        /// <param name="atlas">The Atlas implementation to populate with loaded items.</param>
        /// <param name="options">Optional loading configuration. If null, default options are used.</param>
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
            IProgress<AtlasLoadEvent>? progress = null,
            CancellationToken cancellationToken = default)
        {
            options ??= new AtlasOptions();
            await LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, true, progress, cancellationToken);
            // Invoke in the caller's thread.
            atlas.OnLoaded();
        }

        static async Task LoadAtlasImpl(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions options,
            bool isAsync,
            IProgress<AtlasLoadEvent>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            options.JsonSettings = CreateJsonLoadSettings(options.JsonSettings);

            Func<IFS, string, CancellationToken, Task<byte[]>> readFile = isAsync
                ? (fs, path, ct) => fs.ReadAllBytesAsync(path, ct)
                : (fs, path, _) => Task.FromResult(fs.ReadAllBytes(path));

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

            cancellationToken.ThrowIfCancellationRequested();

            // Read and parse atlas.json
            var atlasData = await readFile(options.FS, atlasFile, cancellationToken);
            AtlasJson? atlasJson;
            try
            {
                atlasJson = JsonConvert.DeserializeObject<AtlasJson>(Encoding.UTF8.GetString(atlasData), options.JsonSettings);
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

            // Load items. IFS is only called on the caller's thread or context: all reads start here, and
            // each item is handed to the thread pool for parsing as soon as its files are read.
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var tasks = new List<Task>(filtered.Count);
                var running = new List<Task>();
                Task? firstFailure = null;

                // Removes finished items. On the first failure, cancels the rest from the caller's thread,
                // so that cancellation callbacks registered by IFS run there too.
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
                            cts.Cancel();
                        }
                    }
                }

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

                    var task = RunItem(kvp.Key, kvp.Value, cts.Token);
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

            async Task RunItem(string key, AtlasItem item, CancellationToken ct)
            {
                try
                {
                    var loading = await ReadItem(key, item, atlasJson, atlasFile, cfgRoot, options, progress, readFile, ct);
                    // Nothing after parsing touches IFS, so there is no need to resume on the caller's context.
                    await Task.Run(() => ParseItem(loading, options, progress, ct), ct).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var msg = $"Failed to load atlas item: \"{key}\". atlasFile: {atlasFile}, cfgRoot: {cfgRoot}";
                    throw new ArchmageException(msg, ex);
                }
            }

            // Bind references
            atlas.BindRefs();

            options.Logger.Info($"<archmage> Loaded {filtered.Count} config items in {stopwatch.ElapsedMilliseconds}ms");
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

        static (string cause, bool skip) ShouldSkip(string key, AtlasOptions options)
        {
            if (options.Whitelist is not null && options.Whitelist.Count > 0)
                return ("whitelist", !options.Whitelist.Contains(key));

            if (options.Blacklist is not null && options.Blacklist.Count > 0 && options.Blacklist.Contains(key))
                return ("blacklist", true);

            return ("", false);
        }

        /// <summary>
        /// Resolves the files of an item from atlas.json. Returns null for an unsupported mapping.
        /// </summary>
        static List<string>? ResolveFiles(string key, AtlasItem item, AtlasJson atlasJson, AtlasOptions options,
            out string keyPath, out string? variant)
        {
            variant = null;
            switch (item.Mapping)
            {
                case AtlasConstants.MappingUnique:
                    keyPath = $"$.unique['{key}']";
                    return atlasJson.Unique.TryGetValue(key, out var uf) ? new List<string> { uf } : new List<string>();
                case AtlasConstants.MappingVariant:
                    variant = options.Variants.TryGetValue(key, out var sv)
                        ? sv : AtlasConstants.VariantMappingDefaultKey;
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
        /// Files of an item that have been read and are waiting to be parsed.
        /// </summary>
        sealed class LoadingItem
        {
            public LoadingItem(AtlasItem item, List<string> filePaths, byte[][] fileData,
                List<(string Path, byte[] Data)> overrides, Stopwatch stopwatch)
            {
                Item = item;
                FilePaths = filePaths;
                FileData = fileData;
                Overrides = overrides;
                Stopwatch = stopwatch;
            }

            public AtlasItem Item { get; }
            public List<string> FilePaths { get; }
            public byte[][] FileData { get; }
            public List<(string Path, byte[] Data)> Overrides { get; }
            public Stopwatch Stopwatch { get; }
        }

        /// <summary>
        /// Reads the files and override files of an item concurrently. Runs on the caller's thread or context.
        /// </summary>
        static async Task<LoadingItem> ReadItem(
            string key,
            AtlasItem item,
            AtlasJson atlasJson,
            string atlasFile,
            string cfgRoot,
            AtlasOptions options,
            IProgress<AtlasLoadEvent>? progress,
            Func<IFS, string, CancellationToken, Task<byte[]>> readFile,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            item.Key = key;

            var files = ResolveFiles(key, item, atlasJson, options, out var keyPath, out var variant)
                ?? throw new Exception($"Unsupported mapping: {item.Mapping}.");
            if (variant is not null)
                item.Variant = variant;

            if (files.Count == 0)
            {
                throw new Exception($"Could not find {keyPath} in {atlasFile}.");
            }

            var stopwatch = Stopwatch.StartNew();

            // Report: StartProcessing
            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartProcessing, elapsed: stopwatch.Elapsed));

            var filePaths = new List<string>(files.Count);
            var fileReads = new List<Task<byte[]>>(files.Count);
            var ovrPaths = new List<string>();
            var ovrReads = new List<Task<byte[]?>>();
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

                    ovrPaths.Add(ovrPath);
                    ovrReads.Add(ReadOptional(fs, ovrPath));
                }
            }

            // Wait for every read, even after one fails, so that no read outlives the load.
            await Task.WhenAll(fileReads.Cast<Task>().Concat(ovrReads));

            var overrides = new List<(string Path, byte[] Data)>(ovrReads.Count);
            for (var i = 0; i < ovrReads.Count; i++)
            {
                var data = ovrReads[i].Result;
                if (data is not null)
                    overrides.Add((ovrPaths[i], data));
            }

            return new LoadingItem(item, filePaths, fileReads.Select(t => t.Result).ToArray(), overrides, stopwatch);

            // Turns a synchronous throw from readFile into a faulted task.
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
            }
        }

        /// <summary>
        /// Deserializes the files of an item and applies its overrides. Runs on a thread pool thread.
        /// </summary>
        static void ParseItem(LoadingItem loading, AtlasOptions options, IProgress<AtlasLoadEvent>? progress,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var item = loading.Item;
            var key = item.Key;
            var stopwatch = loading.Stopwatch;

            for (var i = 0; i < loading.FilePaths.Count; i++)
            {
                // Report: StartParsing
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartParsing, loading.FilePaths[i], stopwatch.Elapsed));

                var json = Encoding.UTF8.GetString(loading.FileData[i]);
                MergeJson(item.Cfg!, json, options.JsonSettings);
            }

            // Apply all overrides
            foreach (var (path, data) in loading.Overrides)
            {
                // Report: ApplyingOverride
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.ApplyingOverride, path, stopwatch.Elapsed));

                var overrideJson = Encoding.UTF8.GetString(data);
                try
                {
                    MergeJson(item.Cfg!, overrideJson, options.JsonSettings);
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
            if (item.Cfg is IApplyKeys applyKeys)
            {
                applyKeys.ApplyKeys();
            }

            // Build log supplement
            var supplement = loading.Overrides.Count switch
            {
                0 => "",
                1 => " with 1 override",
                _ => $" with {loading.Overrides.Count} overrides"
            };

            var paths = string.Join(", ", loading.FilePaths);
            options.Logger.Info($"<archmage> Loaded ({item.Mapping}) {paths}{supplement} ({stopwatch.ElapsedMilliseconds}ms)");
            item.Ready = true;
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
