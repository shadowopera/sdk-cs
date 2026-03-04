using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents the structure of an Atlas index JSON file.
    /// </summary>
    public class AtlasJson
    {
        /// <summary>
        /// Version control information.
        /// </summary>
        [JsonProperty("vcs")]
        public Dictionary<string, object>? Vcs { get; set; }

        /// <summary>
        /// Unique mapping: key -> file path.
        /// </summary>
        [JsonProperty("unique")]
        public Dictionary<string, string> Unique { get; set; } = new();

        /// <summary>
        /// Single mapping: key -> subkey -> file path.
        /// </summary>
        [JsonProperty("single")]
        public Dictionary<string, Dictionary<string, string>> Single { get; set; } = new();

        /// <summary>
        /// Multiple mapping: key -> list of file paths.
        /// </summary>
        [JsonProperty("multiple")]
        public Dictionary<string, List<string>> Multiple { get; set; } = new();

        /// <summary>
        /// Picks the single file path for a key, using the default separator "/".
        /// Returns null if not found.
        /// </summary>
        internal string? PickSingle(string key)
        {
            if (Single.TryGetValue(key, out var subMap) &&
                subMap.TryGetValue(AtlasConstants.DefaultSingleSeparator, out var path))
            {
                return path;
            }
            return null;
        }
    }
}
