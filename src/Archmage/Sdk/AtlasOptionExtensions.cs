#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Extension methods for fluent configuration of AtlasOptions using the builder pattern.
    /// </summary>
    /// <remarks>
    /// All methods return the AtlasOptions instance to enable method chaining.
    /// </remarks>
    public static class AtlasOptionExtensions
    {
        /// <summary>
        /// Sets custom logger (defaults to console).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if logger is null.</exception>
        public static AtlasOptions WithLogger(this AtlasOptions opts, IAtlasLogger logger)
        {
            opts.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return opts;
        }

        /// <summary>
        /// Sets custom filesystem (in-memory, embedded, virtual, etc. — does not affect override sources).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if fs is null.</exception>
        public static AtlasOptions WithFS(this AtlasOptions opts, IFS fs)
        {
            opts.FS = fs ?? throw new ArgumentNullException(nameof(fs));
            return opts;
        }

        /// <summary>
        /// Registers callback to modify atlas.json after loading.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if modifier is null.</exception>
        public static AtlasOptions WithAtlasModifier(this AtlasOptions opts, Action<AtlasJson> modifier)
        {
            opts.AtlasModifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
            return opts;
        }

        /// <summary>
        /// Specifies whitelist (if set, blacklist ignored; loads only whitelisted items).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if whitelist is null.</exception>
        public static AtlasOptions WithWhitelist(this AtlasOptions opts, IEnumerable<string> whitelist)
        {
            opts.Whitelist = whitelist?.ToList() ?? throw new ArgumentNullException(nameof(whitelist));
            return opts;
        }

        /// <summary>
        /// Specifies blacklist (ignored if whitelist present; skips blacklisted items).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if blacklist is null.</exception>
        public static AtlasOptions WithBlacklist(this AtlasOptions opts, IEnumerable<string> blacklist)
        {
            opts.Blacklist = blacklist?.ToList() ?? throw new ArgumentNullException(nameof(blacklist));
            return opts;
        }

        /// <summary>
        /// Adds directory as override source (processed in order; each can override previous).
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if rootPath is null or whitespace.</exception>
        public static AtlasOptions WithOverrideRoot(this AtlasOptions opts, string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Override root path cannot be empty.", nameof(rootPath));

            opts.OverrideConfigs.Add(new OverrideConfig(rootPath));
            return opts;
        }

        /// <summary>
        /// Adds custom filesystem as override source (embedded, network, in-memory, etc.).
        /// When rootPath is specified, override files are resolved relative to that path within the filesystem.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if fs is null.</exception>
        public static AtlasOptions WithOverrideFS(this AtlasOptions opts, IFS fs, string? rootPath = null)
        {
            if (fs is null)
                throw new ArgumentNullException(nameof(fs));

            opts.OverrideConfigs.Add(new OverrideConfig(fs, rootPath));
            return opts;
        }

        /// <summary>
        /// Sets custom load strategy for parallel/cached/conditional loading (default is sequential).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if strategy is null.</exception>
        public static AtlasOptions WithLoadStrategy(this AtlasOptions opts, AtlasLoadStrategy strategy)
        {
            opts.LoadStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            return opts;
        }

        /// <summary>
        /// Sets custom async load strategy for parallel/cached/conditional loading (default is sequential).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if strategy is null.</exception>
        public static AtlasOptions WithAsyncLoadStrategy(this AtlasOptions opts, AtlasAsyncLoadStrategy strategy)
        {
            opts.AsyncLoadStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            return opts;
        }

        /// <summary>
        /// Sets custom JSON settings.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if settings is null.</exception>
        public static AtlasOptions WithJsonSettings(this AtlasOptions opts, JsonSerializerSettings settings)
        {
            opts.JsonSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            return opts;
        }
    }
}
