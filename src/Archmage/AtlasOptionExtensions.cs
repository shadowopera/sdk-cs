namespace Shadop.Archmage;

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
    /// Sets a custom file reading function.
    /// </summary>
    public static AtlasOptions WithReadFile(this AtlasOptions opts, Func<string, byte[]> readFile)
    {
        opts.ReadFile = readFile ?? throw new ArgumentNullException(nameof(readFile));
        return opts;
    }

    /// <summary>
    /// Sets a custom file existence check function.
    /// </summary>
    public static AtlasOptions WithFileExists(this AtlasOptions opts, Func<string, bool> fileExists)
    {
        opts.FileExists = fileExists ?? throw new ArgumentNullException(nameof(fileExists));
        return opts;
    }

    /// <summary>
    /// Sets a custom directory existence check function.
    /// </summary>
    public static AtlasOptions WithDirectoryExists(this AtlasOptions opts, Func<string, bool> directoryExists)
    {
        opts.DirectoryExists = directoryExists ?? throw new ArgumentNullException(nameof(directoryExists));
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

        opts.OverrideConfigs.Add(new OverrideConfig { RootPath = rootPath });
        return opts;
    }

    /// <summary>
    /// Sets a callback to be invoked when a configuration item is not found.
    /// The callback may set <see cref="AtlasItem.Ready"/> to suppress the not-found warning.
    /// The callback may throw an exception to abort loading.
    /// </summary>
    public static AtlasOptions WithNotFoundCallback(this AtlasOptions opts, Action<string, AtlasItem> callback)
    {
        opts.NotFoundCallback = callback ?? throw new ArgumentNullException(nameof(callback));
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
}
