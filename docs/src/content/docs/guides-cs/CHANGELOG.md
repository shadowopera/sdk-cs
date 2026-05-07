---
title: 'Changelog'
sidebar:
  order: 99
---

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.12.0] - 2026-05-07

### Added

- Logger: Implement `ARCHMAGE_DISABLE_DEFAULT_LOGGER` conditional compilation symbol to allow manual suppression of default console logging (automatically disabled in Unity environments).

### Changed

- SDK: Changed vector (`Vec2/3/4`) and Unity vector types to use object-based JSON serialization instead of arrays, ensuring null deserializes to zero vectors.
- SDK: Added `unmanaged` constraint to vector types and optimized equality comparisons.

## [0.11.0] - 2026-04-24

### Added

- Unity: `Vec2/3/4` extension methods to convert to `UnityEngine.Vector2/3/4` and `Vector2/3Int` with zero-allocation support for 10 numeric types.
- Docs: Documentation for Unity Inspector dropdowns and `ArchmageEditorTools.cs` examples.

## [0.10.0] - 2026-04-10

### Added

- Unity Editor: `CfgIdDrawer<TValue>` abstract base, `IntCfgIdDrawer<TId, TValue>`, `StrCfgIdDrawer<TId>` property drawers, and `EasyDropdown<TValue>` IMGUI dropdown for config ID selection.
- `AtlasLoadEvent.ItemsQueued` stage to report total item count before loading begins.
- `WithOverrideFS`: optional `rootPath` parameter to resolve override files relative to a subdirectory within a custom filesystem.

### Changed

- Unity: flattened SDK files from `Runtime/Sdk/` to `Runtime/`.

## [0.9.0] - 2026-04-07

### Added

- `Rgba` struct with hex string parsing (`#RRGGBB` / `#RRGGBBAA`), equality, `IZero` support, and `RgbaJsonConverter` for Newtonsoft.Json; alpha is omitted from output when `A == 0xFF`.
- Unity: `RgbaExtensions.ToColor()` maps `Rgba` channels [0, 255] to `UnityEngine.Color` [0, 1].

## [0.8.0] - 2026-04-06

### Changed

- Renamed namespace and assembly from `Shadop.Archmage` to `Shadop.Archmage.Sdk`.

## [0.7.0] - 2026-04-01

### Added

- Docs: GitHub Package Manager git URL as primary Unity install method.

### Changed

- Renamed `XRef.RawValue` to `XRef.CfgId`.

## [0.6.0] - 2026-03-29

### Changed

- Unity: Removed `Samples~` directory; integration adapters are now first-class Runtime assemblies.
- Config tables now include `Id` in JSON serialization output.

## [0.5.0] - 2026-03-22

### Added

- Added `NullLogger` that discards all log output.

## [0.4.0] - 2026-03-18

### Added

- Added `UnityJsonSettingsFactory` for JSON serialization of Unity types.

### Changed

- Refactored JSON settings handling: renamed `CreateJsonSerializerSettings()` to `CreateJsonDumpSettings()`.

## [0.3.1] - 2026-03-17

### Fixed

- Fixed typo `GetPreferredLanguge` to `GetPreferredLanguage` in `L10n`.

## [0.3.0] - 2026-03-17

### Added

- Added `IZero` interface and `ValueWrapperJsonConverter` / `ValueWrapperTypeConverter` for better primitive wrapper support.
- Added `IFS` support and `MergeL10nFileAsync` variant to `MergeL10nFile` in `I18n`.
- New documentation site built with Astro and Starlight, including comprehensive API docs and C# guides.

### Changed

- Improved `L10n` API: replaced `Text(lang)` with a `Text` property that uses `GetPreferredLanguage`.
- Removed `[Archmage]` prefix from `UnityAtlasLogger` for cleaner logs.

## [0.2.0] - 2026-03-14

### Added

- Unity: `Resources`, `StreamingAssets`, and `Addressables` file system implementations
- Unity: Integration samples and `changelogUrl` added to `package.json`

## [0.1.1] - 2026-03-12

### Added

- NuGet package publishing support: package metadata, Source Link, and XML documentation generation

### Changed

- Clarified override merge rules in documentation
- Updated OpenUPM scoped registry configuration in installation docs

## [0.1.0] - 2026-03-11

### Added

- Initial release
- Atlas loading system with sync/async support, layered overrides, and whitelist/blacklist filtering
- JSON serialization via Newtonsoft.Json (com.unity.nuget.newtonsoft-json)
- Duration type with nanosecond precision
- Vec2, Vec3, Vec4 typed vectors
- Tup1–Tup7 heterogeneous tuples
- XRef type for cross-table references
- I18n internationalization with language fallback
- DateTimeOffset JSON converter
