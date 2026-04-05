#nullable enable

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Constants used by the Atlas system.
    /// </summary>
    public static class AtlasConstants
    {
        /// <summary>
        /// Key for the default file in a MappingSingle group.
        /// </summary>
        public const string SingleMappingDefaultKey = "/";

        /// <summary>
        /// Indicates one-to-one mapping between key and file.
        /// </summary>
        public const string MappingUnique = "unique";

        /// <summary>
        /// Indicates that a key maps to a single file from a set of files.
        /// </summary>
        public const string MappingSingle = "single";

        /// <summary>
        /// Indicates that a key maps to multiple files loaded and merged as one.
        /// </summary>
        public const string MappingMultiple = "multiple";
    }
}
