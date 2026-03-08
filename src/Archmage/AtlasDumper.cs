using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Writes all loaded atlas items to JSON files in outputDir.
        /// Each item is written to a separate file named {key}.json.
        /// </summary>
        /// <param name="atlas">The Atlas instance to dump.</param>
        /// <param name="outputDir">The directory to write JSON files to.</param>
        /// <param name="settings">Optional JSON serializer settings. If null, uses default settings with custom converters.</param>
        public static void DumpAtlas(IAtlas atlas, string outputDir, JsonSerializerSettings? settings = null)
        {
            if (atlas == null)
                throw new ArgumentNullException(nameof(atlas));
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new ArgumentException("Output directory cannot be empty.", nameof(outputDir));

            // Setup default settings with custom converters
            settings ??= CreateDumpSettings();

            var items = atlas.AtlasItems();

            foreach (var kvp in items)
            {
                var key = kvp.Key;
                var item = kvp.Value;

                // Skip items that are not ready or have no configuration
                if (!item.Ready || item.Cfg == null)
                    continue;

                // Serialize item to JSON
                var json = JsonConvert.SerializeObject(item.Cfg, settings);

                // Write to file
                var filePath = Path.Combine(outputDir, key + ".json");
                var dir = Path.GetDirectoryName(filePath);
                if (dir != null)
                    Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, json + "\n");
            }
        }

        static JsonSerializerSettings CreateDumpSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            // Register custom converters (zero-value Vec writes null, matching Go behavior)
            // Do not convert zero Vec2/3/4 to null.
            settings.Converters.Add(new DurationJsonConverter());
            settings.Converters.Add(new RefJsonConverter());

            return settings;
        }
    }
}
