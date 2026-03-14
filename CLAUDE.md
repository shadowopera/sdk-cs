# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the library
dotnet build src/Archmage/Archmage.csproj

# Run all tests
dotnet test tests/Archmage.Tests.csproj

# Run a single test by name
dotnet test tests/Archmage.Tests.csproj --filter "FullyQualifiedName~TestName"

# Regenerate golden files after intentional output changes
UPDATE_GOLDEN=1 dotnet test tests/Archmage.Tests.csproj

# Sync source to Unity package
bash scripts/sync-unity.sh

# Bump version
bash scripts/bump-version.sh [--yes] <version>  # e.g. 0.2.0

# Step-driven release workflow
bash scripts/release.sh [<version>]
```

## Architecture

**Archmage** is a C# configuration management SDK (namespace `Shadop.Archmage`) for loading JSON-based game configs, targeting both .NET (`net8.0`, `netstandard2.1`) and Unity. The core library lives in `src/Archmage/`; the `unity/dev.shadop.archmage/Runtime/` directory is a mirror synced via `scripts/sync-unity.sh`.

### Entry Point

`Archmage` (static class) exposes `LoadAtlas()` / `LoadAtlasAsync()`, both configured via `AtlasOptions` (fluent builder using extension methods in `AtlasOptionExtensions.cs`).

### Atlas Loading Flow

1. Read and parse `atlas.json` — defines three mapping strategies:
   - **`unique`**: key → file path (one-to-one)
   - **`single`**: key → `{case → file path}` (conditional; use `"/"` as default case)
   - **`multiple`**: key → `[file paths]` (list; files merged in order)
2. Apply any registered `AtlasModifier` callbacks to the parsed atlas data
3. For each config item: read files via `IFS`, deserialize, merge JSON, then apply overrides
4. Call `IAtlas.BindRefs()` to resolve cross-table references
5. Call `IAtlas.OnLoaded()` for post-load initialization

### Key Abstractions

| Interface/Class | Role |
|---|---|
| `IAtlas` | Config collection with lifecycle hooks; implemented by generated code in `tests/Conf/` |
| `IFS` | File system abstraction; `DefaultFS` wraps `System.IO` |
| `IAtlasLogger` | Logging; `DefaultLogger` writes to console |
| `AtlasOptions` | Builder for loader configuration (FS, logger, filters, overrides, strategies) |
| `IApplyKeys` | Optional interface on config objects; called after deserialization/overrides, before marking `Ready` |
| `IRefBinder` | Implemented by generated table classes; called during `BindRefs()` to resolve `XRef` fields |

The `Archmage` static class is split across two `partial class` files: `AtlasLoader.cs` (loading logic) and `AtlasDumper.cs` (`DumpAtlas` utility for exporting ready items to JSON files, used for golden file tests).

### Special Types

- **`XRef<V, T>`** — Cross-table reference; raw value stored, resolved in bind phase via `IRefBinder`
- **`Duration`** — Nanosecond-precision duration with compact shard encoding; custom JSON converter
- **`I18n`** — Multi-language text with fallback; loaded from locale JSON files
- **`Vec2/3/4<T>`**, **`Tup1–7`** — Typed vectors and tuples for structured config fields

### Generated Config Pattern

`tests/Conf/` shows the expected shape of user-generated code (files are marked "DO NOT EDIT" — they represent output of the archmage code-generation tool):

- Table classes implement `IRefBinder` and populate themselves via JSON deserialization
- `AtlasExtension.cs` wires all tables to `IAtlas.BindRefs()`
- `L10n.cs` wraps `I18n` for localization lookup
- `Atlas.cs` is the `ConfigAtlas : IAtlas` root; its `BuildMap()` registers each table with its key and mapping type

Tests use golden files under `tests/golden/`. Run `UPDATE_GOLDEN=1 dotnet test` to regenerate them when output changes are intentional.

### Dependencies

- `Newtonsoft.Json 13.0.4` — JSON serialization with custom converters (`XRefJsonConverter`, `DurationJsonConverter`, `VecJsonConverter`)
- `xunit.v3 2.0.3` — Test framework
- C# 9.0, nullable enabled, implicit usings disabled

### Release & CI

- **`scripts/bump-version.sh`** — bumps `<Version>` in `Archmage.csproj` and `unity/.../package.json`, commits, and creates an annotated git tag; use `--yes` to skip interactive prompts
- **`scripts/release.sh`** + **`scripts/release.go`** — step-driven release automation; state persisted in `scripts/release.json`; steps: checkVersion → runTests1 → updateChangelog → syncUnity → runTests2 → bumpVersion
- **`scripts/reconcile-unity-meta.sh`** — checks Unity `.meta` file consistency
- **`.github/workflows/publish-nuget.yml`** — publishes to NuGet on `v*` tag push; verifies tag version matches `Archmage.csproj`, runs tests, packs, and pushes with `NUGET_API_KEY` secret
