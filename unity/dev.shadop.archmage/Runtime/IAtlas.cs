#nullable enable

using System.Collections.Generic;

namespace Shadop.Archmage
{
    /// <summary>
    /// Configuration collection managed by the Atlas loading system.
    /// </summary>
    /// <remarks>
    /// Implementations store configuration items, resolve cross-table references via BindRefs,
    /// and perform post-load initialization in OnLoaded.
    /// </remarks>
    public interface IAtlas
    {
        /// <summary>
        /// Stores VCS version metadata from the atlas.json.
        /// </summary>
        void SetDataVersion(VersionInfo? v);

        /// <summary>
        /// Gets all configuration items registered in this Atlas.
        /// </summary>
        Dictionary<string, AtlasItem> AtlasItems();

        /// <summary>
        /// Resolves cross-table references after all items have been loaded.
        /// </summary>
        void BindRefs();

        /// <summary>
        /// Called after all items are loaded and references are bound.
        /// Perform validation, extension initialization, or throw to abort loading.
        /// </summary>
        void OnLoaded();
    }
}
