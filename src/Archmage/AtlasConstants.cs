namespace Shadop.Archmage
{
    /// <summary>
    /// Constants used by the Atlas system.
    /// </summary>
    public static class AtlasConstants
    {
        /// <summary>
        /// Key for the default file in a single mapping group.
        /// </summary>
        public const string SingleMappingDefaultKey = "/";

        /// <summary>
        /// Mapping type for unique items (single file).
        /// </summary>
        public const string MappingUnique = "unique";

        /// <summary>
        /// Mapping type for single items (one file per key).
        /// </summary>
        public const string MappingSingle = "single";

        /// <summary>
        /// Mapping type for multiple items (multiple files per key).
        /// </summary>
        public const string MappingMultiple = "multiple";
    }
}
