using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Extension methods for fluent configuration of AtlasOptions.
    /// </summary>
    public static class AtlasOptionExtensions
    {
        /// <summary>
        /// Sets a custom logger for Atlas loading.
        /// </summary>
        public static AtlasOptions WithLogger(this AtlasOptions opts, IAtlasLogger logger)
        {
            opts.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return opts;
        }

        /// <summary>
        /// Sets a custom file system implementation for Atlas loading.
        /// </summary>
        public static AtlasOptions WithFS(this AtlasOptions opts, IFS fs)
        {
            opts.FS = fs ?? throw new ArgumentNullException(nameof(fs));
            return opts;
        }

        /// <summary>
        /// Sets a modifier function to customize the AtlasJson after loading.
        /// </summary>
        public static AtlasOptions WithAtlasModifier(this AtlasOptions opts, Action<AtlasJson> modifier)
        {
            opts.AtlasModifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
            return opts;
        }

        /// <summary>
        /// Sets a whitelist of configuration keys to load. Only items in this list will be loaded.
        /// </summary>
        public static AtlasOptions WithWhitelist(this AtlasOptions opts, IEnumerable<string> whitelist)
        {
            opts.Whitelist = whitelist?.ToList() ?? throw new ArgumentNullException(nameof(whitelist));
            return opts;
        }

        /// <summary>
        /// Sets a blacklist of configuration keys to skip. Items in this list will not be loaded.
        /// If a whitelist is also specified, the blacklist will be ignored.
        /// </summary>
        public static AtlasOptions WithBlacklist(this AtlasOptions opts, IEnumerable<string> blacklist)
        {
            opts.Blacklist = blacklist?.ToList() ?? throw new ArgumentNullException(nameof(blacklist));
            return opts;
        }

        /// <summary>
        /// Adds an override directory. Files in this directory will be merged over base configurations.
        /// </summary>
        public static AtlasOptions WithOverrideRoot(this AtlasOptions opts, string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Override root path cannot be empty.", nameof(rootPath));

            opts.OverrideConfigs.Add(new OverrideConfig(rootPath));
            return opts;
        }

        /// <summary>
        /// Adds a filesystem to search for override JSON files that will be merged into loaded configurations.
        /// </summary>
        public static AtlasOptions WithOverrideFS(this AtlasOptions opts, IFS fs)
        {
            if (fs == null)
                throw new ArgumentNullException(nameof(fs));

            opts.OverrideConfigs.Add(new OverrideConfig(fs));
            return opts;
        }

        /// <summary>
        /// Sets a custom loader function for parallel or custom loading strategies.
        /// </summary>
        public static AtlasOptions WithCustomLoader(this AtlasOptions opts, AtlasItemLoader loader)
        {
            opts.CustomLoader = loader ?? throw new ArgumentNullException(nameof(loader));
            return opts;
        }

        /// <summary>
        /// Sets custom JSON serializer settings for deserialization during Atlas loading.
        /// Prevents interference from global <see cref="JsonConvert.DefaultSettings"/>.
        /// </summary>
        public static AtlasOptions WithJsonSettings(this AtlasOptions opts, JsonSerializerSettings settings)
        {
            opts.JsonSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            return opts;
        }
    }
}
