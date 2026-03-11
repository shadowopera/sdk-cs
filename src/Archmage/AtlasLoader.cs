#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shadop.Archmage
{
    /// <summary>
    /// Provides core utility functions for Atlas loading and configuration management.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Loads an Atlas synchronously from the specified index file and root directory.
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
            if (options.CustomAsyncLoader != null)
                throw new ArchmageException("Cannot use WithCustomAsyncLoader with synchronous LoadAtlas.");
            LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, false, progress).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Loads an Atlas asynchronously with progress reporting and cancellation support.
        /// </summary>
        /// <remarks>
        /// <para>This method performs the same operations as LoadAtlas but asynchronously, providing non-blocking I/O.
        /// Progress events are reported via the IProgress interface to allow UI updates and status tracking.
        /// Loading can be cancelled via the CancellationToken.</para>
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
            if (options.CustomLoader != null)
                throw new ArchmageException("Cannot use WithCustomLoader with asynchronous LoadAtlasAsync.");
            await LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, true, progress, cancellationToken);
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
            options.JsonSettings = CloneAndPrepareSettings(options.JsonSettings);

            Func<IFS, string, Task<byte[]>> readFile = isAsync
                ? (fs, path) => fs.ReadAllBytesAsync(path, cancellationToken)
                : (fs, path) => Task.FromResult(fs.ReadAllBytes(path));

            // Verify override directories exist
            foreach (var overrideConfig in options.OverrideConfigs)
            {
                if (overrideConfig.FS == null)
                {
                    if (!options.FS.DirectoryExists(overrideConfig.RootPath!))
                        throw new ArchmageException($"Invalid override root directory \"{overrideConfig.RootPath}\".");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Read and parse atlas.json
            var atlasData = await readFile(options.FS, atlasFile).ConfigureAwait(false);
            AtlasJson? atlasJson;
            try
            {
                atlasJson = JsonConvert.DeserializeObject<AtlasJson>(Encoding.UTF8.GetString(atlasData), options.JsonSettings);
            }
            catch (JsonException ex)
            {
                throw new ArchmageException($"Invalid \"{atlasFile}\".", ex);
            }

            if (atlasJson == null)
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
            if (options.Whitelist != null)
            {
                foreach (var v in options.Whitelist)
                {
                    if (!items.ContainsKey(v))
                        throw new ArchmageException($"Atlas whitelist: unknown item \"{v}\".");
                }
            }
            if (options.Blacklist != null)
            {
                foreach (var v in options.Blacklist)
                {
                    if (!items.ContainsKey(v))
                        throw new ArchmageException($"Atlas blacklist: unknown item \"{v}\".");
                }
            }

            // Sort by key (case-insensitive) and filter
            var sortedKeys = items.Keys
                .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var filteredItems = new List<KeyValuePair<string, AtlasItem>>();
            foreach (var k in sortedKeys)
            {
                var (cause, skip) = ShouldSkip(k, options);
                if (skip)
                {
                    options.Logger.Info($"<archmage> skipping atlas item: {k}. cause: {cause}");
                    continue;
                }
                filteredItems.Add(new KeyValuePair<string, AtlasItem>(k, items[k]));
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Load items
            if (isAsync)
            {
                async Task itemLoadFuncAsync(string key, AtlasItem item, CancellationToken ct)
                {
                    ct.ThrowIfCancellationRequested();
                    try
                    {
                        await LoadItem(key, item, atlasJson, atlasFile, cfgRoot, options, progress, readFile)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        var msg = $"Failed to load atlas item: \"{key}\". atlasFile: {atlasFile}, cfgRoot: {cfgRoot}";
                        throw new ArchmageException(msg, ex);
                    }
                }

                if (options.CustomAsyncLoader != null)
                {
                    await options.CustomAsyncLoader(filteredItems, itemLoadFuncAsync, cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    foreach (var kvp in filteredItems)
                    {
                        await itemLoadFuncAsync(kvp.Key, kvp.Value, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                void itemLoadFunc(string key, AtlasItem item)
                {
                    try
                    {
                        LoadItem(key, item, atlasJson, atlasFile, cfgRoot, options, progress, readFile)
                            .GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        var msg = $"Failed to load atlas item: \"{key}\". atlasFile: {atlasFile}, cfgRoot: {cfgRoot}";
                        throw new ArchmageException(msg, ex);
                    }
                }

                if (options.CustomLoader != null)
                {
                    options.CustomLoader(filteredItems, itemLoadFunc);
                }
                else
                {
                    foreach (var kvp in filteredItems)
                    {
                        itemLoadFunc(kvp.Key, kvp.Value);
                    }
                }
            }

            // Bind references
            atlas.BindRefs();

            // Invoke OnLoaded
            atlas.OnLoaded();
        }

        static JsonSerializerSettings CloneAndPrepareSettings(JsonSerializerSettings? originalSettings)
        {
            JsonSerializerSettings settings;
            if (originalSettings != null)
            {
                settings = new JsonSerializerSettings();
                foreach (var prop in typeof(JsonSerializerSettings).GetProperties())
                {
                    if (prop.CanWrite)
                        prop.SetValue(settings, prop.GetValue(originalSettings));
                }
            }
            else
            {
                settings = new JsonSerializerSettings();
            }

            settings.DateParseHandling = DateParseHandling.None;
            return settings;
        }

        static (string cause, bool skip) ShouldSkip(string key, AtlasOptions options)
        {
            if (options.Whitelist != null && options.Whitelist.Count > 0)
                return ("whitelist", !options.Whitelist.Contains(key));

            if (options.Blacklist != null && options.Blacklist.Count > 0 && options.Blacklist.Contains(key))
                return ("blacklist", true);

            return ("", false);
        }

        static async Task LoadItem(
            string key,
            AtlasItem item,
            AtlasJson atlasJson,
            string atlasFile,
            string cfgRoot,
            AtlasOptions options,
            IProgress<AtlasLoadEvent>? progress,
            Func<IFS, string, Task<byte[]>> readFile)
        {
            item.Key = key;

            // Resolve files based on mapping type
            List<string> files;
            string keyPath;
            switch (item.Mapping)
            {
                case AtlasConstants.MappingUnique:
                    files = atlasJson.Unique.TryGetValue(key, out var uf) ? new List<string> { uf } : new List<string>();
                    keyPath = $"$.unique['{key}']";
                    break;
                case AtlasConstants.MappingSingle:
                    var sf = atlasJson.PickFromSingle(key);
                    files = sf != null ? new List<string> { sf } : new List<string>();
                    keyPath = $"$.single['{key}']['/']";
                    break;
                case AtlasConstants.MappingMultiple:
                    files = atlasJson.Multiple.TryGetValue(key, out var mf) ? mf : new List<string>();
                    keyPath = $"$.multiple['{key}']";
                    break;
                default:
                    throw new Exception($"Unsupported mapping: {item.Mapping}.");
            }

            if (files.Count == 0)
            {
                throw new Exception($"Cannot find {keyPath} in {atlasFile}.");
            }

            var overrideFiles = new List<string>();
            var overrides = new List<byte[]>();
            var paths = new StringBuilder();
            var stopwatch = Stopwatch.StartNew();

            // Report: StartProcessing
            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartProcessing, elapsed: stopwatch.Elapsed));

            // Load each file and collect overrides
            for (var i = 0; i < files.Count; i++)
            {
                var f = files[i];
                var filePath = Path.Combine(cfgRoot, f);

                // Report: StartReading
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartReading, filePath, stopwatch.Elapsed));

                var fileData = await readFile(options.FS, filePath).ConfigureAwait(false);

                // Report: StartParsing
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartParsing, filePath, stopwatch.Elapsed));

                var json = Encoding.UTF8.GetString(fileData);
                MergeJson(item.Cfg!, json, options.JsonSettings);

                // Collect overrides for this file
                foreach (var overrideCfg in options.OverrideConfigs)
                {
                    if (overrideCfg.FS != null)
                    {
                        if (overrideCfg.FS.FileExists(f))
                        {
                            // Report: StartReadingOverride
                            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartReadingOverride, f, stopwatch.Elapsed));

                            overrideFiles.Add(f);
                            overrides.Add(await readFile(overrideCfg.FS, f).ConfigureAwait(false));
                        }
                    }
                    else
                    {
                        var ovrPath = Path.Combine(overrideCfg.RootPath!, f);
                        if (options.FS.FileExists(ovrPath))
                        {
                            // Report: StartReadingOverride
                            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartReadingOverride, ovrPath, stopwatch.Elapsed));

                            overrideFiles.Add(ovrPath);
                            overrides.Add(await readFile(options.FS, ovrPath).ConfigureAwait(false));
                        }
                    }
                }

                if (i > 0) paths.Append(", ");
                paths.Append(filePath);
            }

            // Apply all overrides
            for (var i = 0; i < overrides.Count; i++)
            {
                // Report: ApplyingOverride
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.ApplyingOverride, overrideFiles[i], stopwatch.Elapsed));

                var overrideJson = Encoding.UTF8.GetString(overrides[i]);
                try
                {
                    MergeJson(item.Cfg!, overrideJson, options.JsonSettings);
                }
                catch (JsonException ex)
                {
                    throw new Exception($"Failed to apply override {overrideFiles[i]}.", ex);
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
            var supplement = overrides.Count switch
            {
                0 => "",
                1 => " with 1 override",
                _ => $" with {overrides.Count} overrides"
            };

            options.Logger.Info($"<archmage> loaded ({item.Mapping}) {paths}{supplement} ({stopwatch.ElapsedMilliseconds}ms)");
            item.Ready = true;
        }

        static readonly JsonMergeSettings MergeSettings = new()
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        /// <summary>
        /// Merges JSON values into an existing object using the following rules:
        /// <list type="bullet">
        ///   <item><c>null</c> → resets the target field to its default value or raise an error</item>
        ///   <item>JSON object → recursively merges: only fields present in the input are updated, others remain unchanged</item>
        ///   <item>Any other value → replaces the current value of the target field</item>
        /// </list>
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
                targetContainer.Merge(patchContainer, MergeSettings);
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
