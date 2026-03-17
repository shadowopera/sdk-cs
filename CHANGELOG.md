# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-03-17

### Added

- Added `IZero` interface and `ValueWrapperJsonConverter` / `ValueWrapperTypeConverter` for better primitive wrapper support.
- Added `IFS` support and `MergeL10nFileAsync` variant to `MergeL10nFile` in `I18n`.
- New documentation site built with Astro and Starlight, including comprehensive API docs and C# guides.

### Changed

- Improved `L10n` API: replaced `Text(lang)` with a `Text` property that uses `GetPreferredLanguage`.
- Made `AtlasExtension` and `L10n` classes `partial` to allow user-defined extensions.
- Removed `[Archmage]` prefix from `UnityAtlasLogger` for cleaner logs.

## [0.2.0] - 2026-03-14

### Added

- Unity: `Resources`, `StreamingAssets`, and `Addressables` file system implementations
- Unity: Integration samples and `changelogUrl` added to `package.json`

### Changed

- Renamed `Custom[Async]Loader` to `[Async]LoadStrategy`

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
