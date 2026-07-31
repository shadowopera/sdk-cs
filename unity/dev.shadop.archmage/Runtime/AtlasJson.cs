#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Represents the structure of an Atlas index JSON file (atlas.json).
    /// </summary>
    /// <remarks>
    /// `atlas.json` contains VCS metadata (optional) and three file mapping sections: `unique`, `variant`, and `many`.
    /// </remarks>
    public class AtlasJson
    {
        /// <summary>
        /// VCS metadata (workspace, commit hash, branch, timestamp). May be null if unavailable during build.
        /// </summary>
        [JsonProperty("version")]
        public VersionInfo? Version { get; set; }

        /// <summary>
        /// One-to-one mapping (key → file path).
        /// </summary>
        [JsonProperty("unique")]
        public Dictionary<string, string> Unique { get; set; } = new();

        /// <summary>
        /// One-to-many variant mapping (key → {case → file path}). Use "/" as the default case.
        /// </summary>
        [JsonProperty("variant")]
        public Dictionary<string, Dictionary<string, string>> Variant { get; set; } = new();

        /// <summary>
        /// One-to-many list mapping (key → [file paths]). Files merged in order.
        /// </summary>
        [JsonProperty("many")]
        public Dictionary<string, List<string>> Many { get; set; } = new();

        /// <summary>
        /// Retrieves the file path for the specified key and variant.
        /// </summary>
        /// <param name="key">The item key to look up.</param>
        /// <param name="variant">The variant to look up, "/" being the default one.</param>
        /// <returns>The file path associated with the variant if found; otherwise null.</returns>
        internal string? PickFromVariant(string key, string variant)
        {
            if (Variant.TryGetValue(key, out var subMap) && subMap.TryGetValue(variant, out var path))
                return path;
            return null;
        }
    }
}
