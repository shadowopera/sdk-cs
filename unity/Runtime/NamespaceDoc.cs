// <summary>
// Provides a system for loading and managing configurations from JSON files
// with support for i18n, references, durations, and layered overrides.
//
// The core component is the Atlas system, which loads configuration items from
// an index file and populates them from JSON files. It supports three mapping
// strategies: unique (one-to-one), single (variant-based), and multiple
// (multi-file merging). Configuration overrides can be applied from additional
// directories or custom file system providers.
//
// Key features include:
//   - Atlas-based configuration loading with flexible mapping strategies
//   - I18n for multi-language text management with automatic fallbacks
//   - Ref type for deferred reference resolution between config items
//   - Duration type with compact JSON array format and unit optimization
//   - Vector types (Vec2, Vec3, Vec4) with JSON array serialization
//   - Tuple types (Tup1-7) for grouping mixed-type elements
//
// Example usage:
// <code>
//     var atlas = new ConfigAtlas();
//     Archmage.LoadAtlas("atlas.json", "config", atlas,
//         new AtlasOptions()
//             .WithOverrideRoot("overrides")
//             .WithWhitelist(["item", "hero"]));
// </code>
// </summary>

namespace Shadop.Archmage;
