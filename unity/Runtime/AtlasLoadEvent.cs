using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Stages of Atlas item loading.
    /// </summary>
    public enum AtlasLoadStage
    {
        /// <summary>
        /// Starting to read configuration file(s).
        /// </summary>
        StartReading,

        /// <summary>
        /// Starting to parse JSON.
        /// </summary>
        StartParsing,

        /// <summary>
        /// Applying override file.
        /// </summary>
        ApplyingOverride,

        /// <summary>
        /// Loading completed successfully.
        /// </summary>
        Completed
    }

    /// <summary>
    /// Event data for Atlas loading progress.
    /// </summary>
    public class AtlasLoadEvent
    {
        public AtlasLoadEvent(string key, AtlasLoadStage stage, string? filePath = null, TimeSpan elapsed = default)
        {
            Key = key;
            Stage = stage;
            FilePath = filePath;
            Elapsed = elapsed;
        }

        /// <summary>
        /// The configuration item key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The current loading stage.
        /// </summary>
        public AtlasLoadStage Stage { get; }

        /// <summary>
        /// The file path being processed (if applicable).
        /// </summary>
        public string? FilePath { get; }

        /// <summary>
        /// Time elapsed since the start of this item's loading.
        /// </summary>
        public TimeSpan Elapsed { get; }
    }
}
