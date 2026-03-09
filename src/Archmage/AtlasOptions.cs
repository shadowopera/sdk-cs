using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Configuration options for Atlas loading.
    /// </summary>
    /// <remarks>
    /// To load configurations from non-conventional file systems (e.g., in-memory,
    /// embedded resources, or virtual file systems), use
    /// <see cref="AtlasOptionExtensions.WithFS"/>
    /// to redirect all I/O operations to your custom provider.
    /// </remarks>
    public class AtlasOptions
    {
        internal IAtlasLogger Logger { get; set; } = new DefaultLogger();
        internal IFS FS { get; set; } = new DefaultFS();
        internal List<OverrideConfig> OverrideConfigs { get; set; } = new();
        internal Action<AtlasJson>? AtlasModifier { get; set; }
        internal List<string>? Whitelist { get; set; }
        internal List<string>? Blacklist { get; set; }
        internal AtlasItemLoader? CustomLoader { get; set; }
        internal JsonSerializerSettings? JsonSettings { get; set; }
    }

    /// <summary>
    /// Represents an override configuration directory or filesystem.
    /// </summary>
    readonly struct OverrideConfig
    {
        public OverrideConfig(string rootPath)
        {
            RootPath = rootPath;
            FS = null;
        }

        public OverrideConfig(IFS fs)
        {
            RootPath = null;
            FS = fs;
        }

        public string? RootPath { get; }
        public IFS? FS { get; }
    }

    /// <summary>
    /// Delegate for custom Atlas item loading. Allows users to implement
    /// parallel loading or other custom strategies.
    /// </summary>
    /// <param name="items">The items to load.</param>
    /// <param name="loadFunc">The function to call for each item to perform the actual loading.</param>
    public delegate void AtlasItemLoader(
        IEnumerable<KeyValuePair<string, AtlasItem>> items,
        AtlasItemLoadFunc loadFunc);

    /// <summary>
    /// Delegate for loading a single Atlas item.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <param name="item">The item to load.</param>
    public delegate void AtlasItemLoadFunc(string key, AtlasItem item);
}
