using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Enumeration of stages during the loading of an individual Atlas item.
    /// </summary>
    /// <remarks>
    /// These stages are reported via <see cref="IProgress{AtlasLoadEvent}"/> during async loading
    /// to allow progress tracking and cancellation feedback.
    /// </remarks>
    public enum AtlasLoadStage
    {
        /// <summary>
        /// The loading process is about to read configuration file(s) from the filesystem.
        /// </summary>
        StartReading,

        /// <summary>
        /// File content has been read and is about to be parsed as JSON.
        /// </summary>
        StartParsing,

        /// <summary>
        /// An override file has been loaded and is being merged into the configuration.
        /// </summary>
        ApplyingOverride,

        /// <summary>
        /// The item has been fully loaded, parsed, and all overrides have been applied.
        /// </summary>
        Completed
    }

    /// <summary>
    /// Progress event data reported during asynchronous Atlas item loading.
    /// </summary>
    /// <remarks>
    /// <para>AtlasLoadEvent instances are reported to an <see cref="IProgress{AtlasLoadEvent}"/>
    /// implementation passed to <see cref="Archmage.LoadAtlasAsync(string, string, IAtlas, AtlasOptions?)"/>.
    /// This allows consumers to track loading progress and provide feedback to the user.</para>
    /// <para>The Elapsed property can be used to implement timeouts or progress visualization.</para>
    /// </remarks>
    public readonly struct AtlasLoadEvent
    {
        public AtlasLoadEvent(string key, AtlasLoadStage stage, string? filePath = null, TimeSpan elapsed = default)
        {
            Key = key;
            Stage = stage;
            FilePath = filePath;
            Elapsed = elapsed;
        }

        public string Key { get; }

        public AtlasLoadStage Stage { get; }

        /// <summary>
        /// Set during StartReading, StartParsing, ApplyingOverride; null for Completed.
        /// </summary>
        public string? FilePath { get; }

        public TimeSpan Elapsed { get; }
    }
}
