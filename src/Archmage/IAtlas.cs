namespace Shadop.Archmage;

/// <summary>
/// Interface for Atlas implementations that manage configuration loading.
/// </summary>
public interface IAtlas
{
    /// <summary>
    /// Sets version control information from the Atlas index file.
    /// </summary>
    /// <param name="versionInfo">Version control data (may be null).</param>
    void SetVersionInfo(Dictionary<string, object>? versionInfo);

    /// <summary>
    /// Gets all Atlas items managed by this instance.
    /// </summary>
    /// <returns>Dictionary of item key to AtlasItem.</returns>
    Dictionary<string, AtlasItem> AtlasItems();

    /// <summary>
    /// Binds all references after loading is complete.
    /// Typically calls BindRefs on all IRefBinder implementations.
    /// </summary>
    void BindRefs();

    /// <summary>
    /// Called after all items are loaded and references are bound.
    /// Use this to perform post-load initialization of configuration extensions.
    /// </summary>
    void OnLoaded();
}
