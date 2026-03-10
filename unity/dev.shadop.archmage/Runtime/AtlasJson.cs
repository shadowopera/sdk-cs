using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents the structure of an Atlas index JSON file (atlas.json).
    /// </summary>
    /// <remarks>
    /// The atlas index file contains VCS metadata and three mapping strategies that define how configuration item keys map to JSON files.
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
        /// One-to-many conditional mapping (key → {case → file path}). Use "/" as the default case.
        /// </summary>
        [JsonProperty("single")]
        public Dictionary<string, Dictionary<string, string>> Single { get; set; } = new();

        /// <summary>
        /// One-to-many list mapping (key → [file paths]). Files merged in order.
        /// </summary>
        [JsonProperty("multiple")]
        public Dictionary<string, List<string>> Multiple { get; set; } = new();

        /// <summary>
        /// Retrieves the default file path for a single-mapped key.
        /// </summary>
        /// <param name="key">The item key to look up.</param>
        /// <returns>The default file path (associated with "/") if found; otherwise null.</returns>
        internal string? PickFromSingle(string key)
        {
            if (Single.TryGetValue(key, out var subMap) &&
                subMap.TryGetValue(AtlasConstants.SingleMappingDefaultKey, out var path))
            {
                return path;
            }
            return null;
        }
    }
}
