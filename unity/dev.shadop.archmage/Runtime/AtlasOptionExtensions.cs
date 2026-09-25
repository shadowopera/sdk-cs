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
        /// Sets custom filesystem (in-memory, embedded, virtual, etc.).
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
            opts.Whitelist = whitelist.ToList() ?? throw new ArgumentNullException(nameof(whitelist));
            return opts;
        }

        /// <summary>
        /// Specifies blacklist (ignored if whitelist present; skips blacklisted items).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if blacklist is null.</exception>
        public static AtlasOptions WithBlacklist(this AtlasOptions opts, IEnumerable<string> blacklist)
        {
            opts.Blacklist = blacklist.ToList() ?? throw new ArgumentNullException(nameof(blacklist));
            return opts;
        }

        /// <summary>
        /// Selects the variant to load for the item identified by key (a variant-mapped item
        /// that is not given a variant falls back to "/").
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if key or variant is null.</exception>
        public static AtlasOptions WithVariant(this AtlasOptions opts, string key, string variant)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (variant is null)
                throw new ArgumentNullException(nameof(variant));

            opts.Variants[key] = variant;
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
        /// Sets the maximum number of items loaded at the same time (default 32). An item counts from the start of
        /// reading its files until it is deserialized. This limits open files, memory held by file contents, and
        /// deserialization work queued on the thread pool. Use 1 to load items one at a time.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if n is less than 1.</exception>
        public static AtlasOptions WithMaxConcurrency(this AtlasOptions opts, int n)
        {
            if (n < 1)
                throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least 1.");
            opts.MaxConcurrency = n;
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
