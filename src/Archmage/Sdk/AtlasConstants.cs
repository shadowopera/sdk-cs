#nullable enable

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Constants used by the Atlas system.
    /// </summary>
    public static class AtlasConstants
    {
        /// <summary>
        /// Key for the default file in a MappingVariant group.
        /// </summary>
        public const string VariantMappingDefaultKey = "/";

        /// <summary>
        /// Indicates a one-to-one mapping between a key and a file.
        /// </summary>
        public const string MappingUnique = "unique";

        /// <summary>
        /// Indicates that a key maps to multiple file variants, only one of which is loaded.
        /// </summary>
        public const string MappingVariant = "variant";

        /// <summary>
        /// Indicates that a key maps to multiple files loaded separately and merged into one.
        /// </summary>
        public const string MappingMany = "many";
    }
}
