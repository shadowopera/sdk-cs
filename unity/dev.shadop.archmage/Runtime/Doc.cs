#nullable enable

// <summary>
// The C# SDK is the runtime library through which C# applications load and
// access config data exported by Archmage.
//
// [Archmage](https://shadop.dev/archmage/) is a configuration solution for
// game development: specifications for how to structure config data, define
// fields, and fill in each value; pipelines that export runtime data and
// generate strongly-typed code; multi-language SDKs for loading and accessing
// that data at runtime; and a collaborative editing workflow for teams.
//
// The SDK is built around the concept of an **Atlas** — a registry that maps named
// keys to configurations. Each key is associated with one or more JSON files.
// At runtime, the SDK reads these files, deserializes them into instances of
// generated C# types, resolves cross-table references, and calls post-load hooks.
//
// Key features:
//   - I18n — multi-language text management with automatic fallback
//   - XRef — cross-table reference resolution via `IAtlas.BindRefs`
//   - Duration — nanosecond precision; formats as human-readable strings such as `1s200ms`
//   - MinMax — random value selection within a range
//   - WeightedPool — weighted random selection with probability proportional to item weight
//   - Variants — switch an item to an alternative data set at load time via `WithVariant`
//   - Whitelist/Blacklist — load only a subset of items
//   - Layered overrides — merge files with matching relative paths from
//     additional override sources (a directory path or a custom file system)
//     into the base configs, field by field, at load time
//   - Synchronous and asynchronous loading — progress reporting, cancellation,
//     and pluggable strategies for parallel loading
//   - Pluggable file system — load from embedded resources, in-memory data, or
//     any other source via `IFS`
//   - Versioning — VCS metadata (branch, commit, timestamp, etc.), when present
//     in `atlas.json`, is available on the loaded atlas
//   - Unity support — built-in adapters for Addressables, Resources, and `StreamingAssets`;
//     Inspector dropdowns for config ID fields, populated from the loaded atlas,
//     for easy selection
//
// Example usage:
// <code>
//     var atlas = new ConfigAtlas();
//     Archmage.LoadAtlas("atlas.json", "config", atlas,
//         new AtlasOptions()
//             .WithOverrideRoot("overrides")
//             .WithWhitelist(new[] { "item", "hero" }));
// </code>
// </summary>

namespace Shadop.Archmage.Sdk
{
}
