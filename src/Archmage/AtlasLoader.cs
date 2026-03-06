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
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Loads an Atlas synchronously from the specified index file.
        /// </summary>
        /// <param name="atlasFile">Path to the Atlas index JSON file.</param>
        /// <param name="cfgRoot">Root directory for configuration files.</param>
        /// <param name="atlas">The Atlas instance to populate.</param>
        /// <param name="options">Optional loading options.</param>
        public static void LoadAtlas(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions? options = null)
        {
            options ??= new AtlasOptions();
            LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, null, CancellationToken.None);
            atlas.OnLoaded();
        }

        /// <summary>
        /// Loads an Atlas asynchronously with progress reporting and cancellation support.
        /// </summary>
        public static Task LoadAtlasAsync(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions? options = null,
            IProgress<AtlasLoadEvent>? progress = null,
            CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                options ??= new AtlasOptions();
                LoadAtlasImpl(atlasFile, cfgRoot, atlas, options, progress, cancellationToken);
                atlas.OnLoaded();
            }, cancellationToken);
        }

        static void LoadAtlasImpl(
            string atlasFile,
            string cfgRoot,
            IAtlas atlas,
            AtlasOptions options,
            IProgress<AtlasLoadEvent>? progress,
            CancellationToken cancellationToken)
        {
            // Verify override directories exist
            foreach (var overrideConfig in options.OverrideConfigs)
            {
                if (!options.DirectoryExists(overrideConfig.RootPath))
                    throw new ArchmageException($"invalid override root directory \"{overrideConfig.RootPath}\"");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Read and parse atlas index file
            var atlasData = options.ReadFile(atlasFile);
            var atlasJson = JsonConvert.DeserializeObject<AtlasJson>(Encoding.UTF8.GetString(atlasData), options.JsonSettings)
                ?? throw new ArchmageException($"invalid \"{atlasFile}\"");

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
                        throw new ArchmageException($"atlas whitelist: unknown item \"{v}\"");
                }
            }
            if (options.Blacklist != null)
            {
                foreach (var v in options.Blacklist)
                {
                    if (!items.ContainsKey(v))
                        throw new ArchmageException($"atlas blacklist: unknown item \"{v}\"");
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
            void ItemLoadFunc(string key, AtlasItem item)
            {
                cancellationToken.ThrowIfCancellationRequested();
                LoadItem(key, item, atlasJson, atlasFile, cfgRoot, options, progress);
            }

            if (options.CustomLoader != null)
            {
                options.CustomLoader(filteredItems, ItemLoadFunc);
            }
            else
            {
                foreach (var kvp in filteredItems)
                {
                    ItemLoadFunc(kvp.Key, kvp.Value);
                }
            }

            // Bind references
            atlas.BindRefs();
        }

        static (string cause, bool skip) ShouldSkip(string key, AtlasOptions options)
        {
            if (options.Whitelist != null && options.Whitelist.Count > 0)
                return ("whitelist", !options.Whitelist.Contains(key));

            if (options.Blacklist != null && options.Blacklist.Count > 0 && options.Blacklist.Contains(key))
                return ("blacklist", true);

            return ("", false);
        }

        static void LoadItem(
            string key,
            AtlasItem item,
            AtlasJson atlasJson,
            string atlasFile,
            string cfgRoot,
            AtlasOptions options,
            IProgress<AtlasLoadEvent>? progress)
        {
            var stopwatch = Stopwatch.StartNew();

            item.Key = key;

            // Resolve files based on mapping type
            List<string> files;
            string notFoundHint;
            switch (item.Mapping)
            {
                case AtlasConstants.MappingUnique:
                    files = atlasJson.Unique.TryGetValue(key, out var uf) ? new List<string> { uf } : new List<string>();
                    notFoundHint = $"$.unique['{key}']";
                    break;
                case AtlasConstants.MappingSingle:
                    var sf = atlasJson.PickSingleDefault(key);
                    files = sf != null ? new List<string> { sf } : new List<string>();
                    notFoundHint = $"$.single['{key}']['/']";
                    break;
                case AtlasConstants.MappingMultiple:
                    files = atlasJson.Multiple.TryGetValue(key, out var mf) ? mf : new List<string>();
                    notFoundHint = $"$.multiple['{key}']";
                    break;
                default:
                    throw new ArchmageException($"unsupported mapping: {item.Mapping}");
            }

            if (files.Count == 0)
            {
                options.NotFoundCallback?.Invoke(key, item);
                if (!item.Ready)
                {
                    options.Logger.Warn($"<archmage> cannot find {notFoundHint} in {atlasFile}");
                }
                return;
            }

            // Report: StartReading
            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartReading, elapsed: stopwatch.Elapsed));

            var overrideFiles = new List<string>();
            var overrides = new List<byte[]>();
            var paths = new StringBuilder();

            // Load each file and collect overrides
            for (var i = 0; i < files.Count; i++)
            {
                var f = files[i];
                var filePath = Path.Combine(cfgRoot, f);

                // Report: StartParsing
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.StartParsing, filePath, stopwatch.Elapsed));

                var fileData = options.ReadFile(filePath);
                var json = Encoding.UTF8.GetString(fileData);
                MergeJson(item.Cfg!, json, options.JsonSettings);

                // Collect overrides for this file
                foreach (var overrideCfg in options.OverrideConfigs)
                {
                    var ovrPath = Path.Combine(overrideCfg.RootPath, f);
                    if (options.FileExists(ovrPath))
                    {
                        overrideFiles.Add(ovrPath);
                        overrides.Add(options.ReadFile(ovrPath));
                    }
                }

                if (i > 0) paths.Append(", ");
                paths.Append(filePath);
            }

            // Apply all overrides
            for (var i = 0; i < overrides.Count; i++)
            {
                progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.ApplyingOverride, overrideFiles[i], stopwatch.Elapsed));

                var overrideJson = Encoding.UTF8.GetString(overrides[i]);
                var overrideToken = JToken.Parse(overrideJson);
                Archmage.Merge(item.Cfg!, overrideToken);
            }

            // Call ApplyKeys if implemented
            if (item.Cfg is IApplyKeys applyKeys)
            {
                applyKeys.ApplyKeys();
            }

            stopwatch.Stop();

            // Build log supplement
            var supplement = overrides.Count switch
            {
                0 => "",
                1 => " with 1 override",
                _ => $" with {overrides.Count} overrides"
            };

            // Report: Completed
            progress?.Report(new AtlasLoadEvent(key, AtlasLoadStage.Completed, elapsed: stopwatch.Elapsed));

            options.Logger.Info($"<archmage> loaded ({item.Mapping}) {paths}{supplement} ({stopwatch.ElapsedMilliseconds}ms)");
            item.Ready = true;
        }

        static readonly JsonMergeSettings MergeSettings = new()
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge
        };

        static void MergeJson(object target, string json, JsonSerializerSettings? settings)
        {
            var serial = JsonSerializer.Create(settings);
            var targetToken = JObject.FromObject(target, serial);
            var patch = JObject.Parse(json);
            targetToken.Merge(patch, MergeSettings);
            serial.ObjectCreationHandling = ObjectCreationHandling.Replace;
            using var reader = targetToken.CreateReader();
            serial.Populate(reader, target);
        }
    }

    /// <summary>
    /// Interface for configuration objects that need to populate ID fields from dictionary keys.
    /// </summary>
    public interface IApplyKeys
    {
        /// <summary>
        /// Called after deserialization to populate ID fields or perform other post-load processing.
        /// </summary>
        void ApplyKeys();
    }
}
