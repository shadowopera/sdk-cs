#nullable enable

using System;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Enumeration of stages during Atlas loading.
    /// </summary>
    /// <remarks>
    /// <para>These stages are reported via <see cref="IProgress{AtlasLoadEvent}"/> during async loading
    /// to allow progress tracking and cancellation feedback.</para>
    /// <para><c>ItemsQueued</c> is reported once, before loading any items.
    /// All other stages are reported per item.</para>
    /// </remarks>
    public enum AtlasLoadStage
    {
        /// <summary>
        /// Atlas loading is about to begin. <see cref="AtlasLoadEvent.Total"/> indicates the number of items to load.
        /// </summary>
        ItemsQueued,

        /// <summary>
        /// The loading process is about to process configuration file(s) for the item.
        /// </summary>
        StartProcessing,

        /// <summary>
        /// The loading process is about to read the base configuration file from the filesystem.
        /// </summary>
        StartReading,

        /// <summary>
        /// The base configuration file content has been read and is about to be parsed as JSON.
        /// </summary>
        StartParsing,

        /// <summary>
        /// An override file has been found and is about to be read from the filesystem.
        /// </summary>
        StartReadingOverride,

        /// <summary>
        /// An override file has been read and is being parsed and merged into the configuration.
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
    /// implementation passed to <see cref="Archmage.LoadAtlasAsync"/>.
    /// This allows consumers to track loading progress and provide feedback to the user.</para>
    /// <para>The Elapsed property can be used to implement timeouts or progress visualization.</para>
    /// </remarks>
    public readonly struct AtlasLoadEvent
    {
        public AtlasLoadEvent(string key, AtlasLoadStage stage, string? filePath = null, TimeSpan elapsed = default, int total = 0)
        {
            Key = key;
            Stage = stage;
            FilePath = filePath;
            Elapsed = elapsed;
            Total = total;
        }

        public string Key { get; }

        public AtlasLoadStage Stage { get; }

        /// <summary>
        /// Set during StartReading, StartParsing, StartReadingOverride, and ApplyingOverride; null for StartProcessing and Completed.
        /// </summary>
        public string? FilePath { get; }

        public TimeSpan Elapsed { get; }

        /// <summary>
        /// The total number of atlas items to load. Only meaningful for <see cref="AtlasLoadStage.ItemsQueued"/>.
        /// </summary>
        public int Total { get; }
    }
}
