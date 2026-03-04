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
    /// embedded resources, or virtual file systems), combine
    /// <see cref="AtlasOptionExtensions.WithReadFile"/>,
    /// <see cref="AtlasOptionExtensions.WithFileExists"/>, and
    /// <see cref="AtlasOptionExtensions.WithDirectoryExists"/>
    /// to redirect all I/O operations to your custom provider.
    /// </remarks>
    public class AtlasOptions
    {
        internal IAtlasLogger Logger { get; set; } = new DefaultLogger();
        internal Func<string, byte[]> ReadFile { get; set; } = File.ReadAllBytes;
        internal Func<string, bool> FileExists { get; set; } = File.Exists;
        internal Func<string, bool> DirectoryExists { get; set; } = Directory.Exists;
        internal List<OverrideConfig> OverrideConfigs { get; set; } = new();
        internal Action<AtlasJson>? AtlasModifier { get; set; }
        internal Action<string, AtlasItem>? NotFoundCallback { get; set; }
        internal List<string>? Whitelist { get; set; }
        internal List<string>? Blacklist { get; set; }
        internal AtlasItemLoader? CustomLoader { get; set; }
        internal JsonSerializerSettings? JsonSettings { get; set; }
    }

    /// <summary>
    /// Represents an override configuration directory.
    /// </summary>
    internal class OverrideConfig
    {
        public OverrideConfig(string rootPath)
        {
            RootPath = rootPath;
        }

        public string RootPath { get; }
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
