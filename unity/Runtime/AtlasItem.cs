namespace Shadop.Archmage
{
    /// <summary>
    /// Represents a single configuration item in the Atlas.
    /// </summary>
    public class AtlasItem
    {
        /// <summary>
        /// The deserialized configuration object.
        /// </summary>
        public object? Cfg { get; set; }

        /// <summary>
        /// The mapping type: "unique", "single", or "multiple".
        /// </summary>
        public string Mapping { get; set; } = string.Empty;

        /// <summary>
        /// The item key.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether this item has been successfully loaded.
        /// </summary>
        public bool Ready { get; set; }
    }
}
