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

# Sync source to Unity package
bash scripts/sync-unity.sh
```

## Architecture

**Archmage** is a C# configuration management SDK (namespace `Shadop.Archmage`) for loading JSON-based game configs, targeting both .NET (`net8.0`, `netstandard2.1`) and Unity. The core library lives in `src/Archmage/`; the `unity/Runtime/` directory is a mirror synced via `scripts/sync-unity.sh`.

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
| `AtlasOptions` | Builder for loader configuration (FS, logger, filters, overrides, loaders) |
| `AtlasLoader` | Unified sync/async loading implementation |

### Special Types

- **`XRef<V, T>`** — Cross-table reference; raw value stored, resolved in bind phase via `IRefBinder`
- **`Duration`** — Nanosecond-precision duration with compact shard encoding; custom JSON converter
- **`I18n`** — Multi-language text with fallback; loaded from locale JSON files
- **`Vec2/3/4<T>`**, **`Tup1–7`** — Typed vectors and tuples for structured config fields

### Generated Config Pattern

`tests/Conf/` shows the expected shape of user-generated code:

- Table classes implement `IRefBinder` and populate themselves via JSON deserialization
- `AtlasExtension.cs` wires all tables to `IAtlas.BindRefs()`
- `L10n.cs` wraps `I18n` for localization lookup

### Dependencies

- `Newtonsoft.Json 13.0.4` — JSON serialization with custom converters (`XRefJsonConverter`, `DurationJsonConverter`, `VecJsonConverter`)
- `xunit.v3 2.0.3` — Test framework
- C# 9.0, nullable enabled, implicit usings disabled
