using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Configuration options for Atlas loading operations.
    /// </summary>
    /// <remarks>
    /// <para>AtlasOptions provides a fluent builder pattern for configuring how the Atlas system loads and processes configuration files.
    /// Use the extension methods in <see cref="AtlasOptionExtensions"/> to configure various aspects of loading.</para>
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
        internal AtlasItemAsyncLoader? CustomAsyncLoader { get; set; }
        internal JsonSerializerSettings? JsonSettings { get; set; }
    }

    /// <summary>
    /// Override source (directory path or custom filesystem). Matched files merged into base config.
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

        /// <summary>
        /// Directory path for file-based overrides; null for custom filesystem.
        /// </summary>
        public string? RootPath { get; }

        /// <summary>
        /// Custom filesystem for overrides; null for directory-based.
        /// </summary>
        public IFS? FS { get; }
    }

    /// <summary>
    /// Delegate for custom loading strategies (parallel, caching, conditional; default is sequential).
    /// Must call loadFunc for each item.
    /// </summary>
    public delegate void AtlasItemLoader(
        IEnumerable<KeyValuePair<string, AtlasItem>> items,
        AtlasItemLoadFunc loadFunc);

    /// <summary>
    /// Callback for loading single item (reads file(s), deserializes JSON, merges overrides).
    /// </summary>
    public delegate void AtlasItemLoadFunc(string key, AtlasItem item);

    /// <summary>
    /// Delegate for custom asynchronous loading strategies (parallel, caching, conditional; default is sequential).
    /// Must call loadFunc for each item.
    /// </summary>
    public delegate Task AtlasItemAsyncLoader(
        IEnumerable<KeyValuePair<string, AtlasItem>> items,
        AtlasItemAsyncLoadFunc loadFunc,
        CancellationToken cancellationToken);

    /// <summary>
    /// Callback for asynchronously loading single item (reads file(s), deserializes JSON, merges overrides).
    /// </summary>
    public delegate Task AtlasItemAsyncLoadFunc(string key, AtlasItem item, CancellationToken cancellationToken);
}
