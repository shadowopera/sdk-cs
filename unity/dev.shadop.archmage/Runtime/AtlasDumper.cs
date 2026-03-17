#nullable enable

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Exports Atlas items to {key}.json files (only Ready items; pretty-printed).
        /// </summary>
        /// <remarks>
        /// Useful for debugging, testing (golden files), and human-readable export.
        /// Output uses custom converters for Duration/XRef types.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if atlas or outputDir is null.</exception>
        /// <exception cref="ArgumentException">Thrown if outputDir is empty or whitespace.</exception>
        public static void DumpAtlas(IAtlas atlas, string outputDir, JsonSerializerSettings? settings = null)
        {
            if (atlas == null)
                throw new ArgumentNullException(nameof(atlas));
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new ArgumentException("Output directory cannot be empty.", nameof(outputDir));

            // Setup default settings with custom converters
            settings = CreateJsonDumpSettings(settings);
            var items = atlas.AtlasItems();

            foreach (var kvp in items)
            {
                var key = kvp.Key;
                var item = kvp.Value;

                // Skip items that are not ready or have no configuration
                if (!item.Ready || item.Cfg == null)
                    continue;

                try
                {
                    DumpAtlasItem(key, item, outputDir, settings);
                }
                catch (Exception ex)
                {
                    throw new ArchmageException($"Failed to dump atlas item \"{key}\".", ex);
                }
            }
        }

        static void DumpAtlasItem(string key, AtlasItem item, string outputDir, JsonSerializerSettings? settings)
        {
            // Serialize item to JSON with LF line endings
            var sb = new System.Text.StringBuilder();
            using var sw = new System.IO.StringWriter(sb) { NewLine = "\n" };
            using var jw = new JsonTextWriter(sw) { IndentChar = ' ', Indentation = 2 };
            JsonSerializer.Create(settings).Serialize(jw, item.Cfg);
            sb.Append('\n');
            var json = sb.ToString();

            // Write to file
            var filePath = Path.Combine(outputDir, key + ".json");
            var dir = Path.GetDirectoryName(filePath);
            if (dir != null)
                Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, json, new UTF8Encoding(false));
        }

        /// <summary>
        /// Creates JSON settings for Atlas export (indented, include nulls/defaults, custom converters).
        /// </summary>
        public static JsonSerializerSettings CreateJsonDumpSettings(JsonSerializerSettings? baseSettings = null)
        {
            var settings = CloneJsonSettings(baseSettings);
            settings.Formatting = Formatting.Indented;
            settings.NullValueHandling = NullValueHandling.Include;
            settings.DefaultValueHandling = DefaultValueHandling.Include;
            settings.Converters.Add(new DateTimeOffsetJsonConverter());
            return settings;
        }
    }
}
