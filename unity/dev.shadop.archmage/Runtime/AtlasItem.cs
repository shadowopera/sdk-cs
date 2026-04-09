#nullable enable

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Configuration item within an Atlas.
    /// </summary>
    public class AtlasItem
    {
        /// <summary>
        /// The deserialized configuration object. Initialize to target type instance before loading.
        /// </summary>
        public object? Cfg { get; set; }

        /// <summary>
        /// The mapping strategy: "unique", "single", or "multiple".
        /// </summary>
        public string Mapping { get; set; } = string.Empty;

        /// <summary>
        /// The item's key in atlas.json.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Whether this item has been successfully loaded.
        /// </summary>
        public bool Ready { get; set; }
    }
}
