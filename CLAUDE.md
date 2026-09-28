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
scripts/rsync-unity.sh

# Run ArchmageDev PlayMode tests in batch mode (Unity Editor must be closed)
# --packed builds Addressables content and loads it from bundles ("Use Existing Build")
# Summary: unity/ArchmageDev/Logs/playmode-<assetdb|packed>-summary.txt
scripts/unity-test.sh [--no-sync] [--packed] [--filter <expr>]

# Run .NET tests, then Unity PlayMode tests in both modes (args pass through to unity-test.sh)
scripts/run-all-tests.sh

# Run PlatformProbe cases on a real platform (Unity Editor must be closed; cases: unity/ArchmageDev/Assets/PlatformProbe/PlatformProbe.cs)
scripts/webgl-probe.sh [--no-build] [<case>...]                                  # headless Chrome
scripts/android-probe.sh [--no-build] [--format apk|aab|aab-split] [<case>...]   # adb + logcat

# Start the Android emulator used by android-probe.sh (AVD archmage-probe: API 35, arm64-v8a)
~/Library/Android/sdk/emulator/emulator -avd archmage-probe -no-window -no-audio &

# Bump version
scripts/bump-version.sh [--yes] <version>  # e.g. 0.2.0

# Step-driven release workflow
scripts/release.sh [<version>]
```

## Architecture

**Archmage** is a C# configuration management SDK (namespace `Shadop.Archmage.Sdk`) for loading JSON-based game configs, targeting both .NET (`net8.0`, `netstandard2.1`) and Unity. The core library lives in `src/Archmage/`; the `unity/dev.shadop.archmage/Runtime/` directory is a mirror synced via `scripts/rsync-unity.sh`.

### Unity Package Structure

The Unity package (`unity/dev.shadop.archmage/`) uses three assemblies:

| Assembly | asmdef | Source | Notes |
|---|---|---|---|
| `Shadop.Archmage.Sdk` | `Runtime/Shadop.Archmage.Sdk.asmdef` | `src/Archmage/Sdk/*.cs` | Pure C#, `noEngineReferences: true` |
| `Shadop.Archmage.Sdk.Unity` | `Runtime/Unity/Shadop.Archmage.Sdk.Unity.asmdef` | `src/Archmage/Sdk/Unity/*.cs` | Unity engine adapters (`UnityResourcesFS`, `UnityStreamingAssetsFS`, `UnityAtlasLogger`) |
| `Shadop.Archmage.Sdk.Unity.Addressables` | `Runtime/Unity/Addressables/Shadop.Archmage.Sdk.Unity.Addressables.asmdef` | `src/Archmage/Sdk/Unity/Addressables/*.cs` | Addressables adapter; only compiled when `com.unity.addressables` is installed (`defineConstraints: ["UNITY_ADDRESSABLES"]`) |

`Shadop.Archmage.Sdk.Unity` and `Shadop.Archmage.Sdk.Unity.Addressables` both reference `Shadop.Archmage.Sdk`. The asmdef files are not synced by rsync — they live directly in the Unity package directory.

### Entry Point

`Archmage` (static class) exposes `LoadAtlas()` / `LoadAtlasAsync()`, both configured via `AtlasOptions` (fluent builder using extension methods in `AtlasOptionExtensions.cs`).

### Atlas Loading Flow

1. Read and parse `atlas.json` — defines three mapping strategies:
   - **`unique`**: key → file path (one-to-one)
   - **`variant`**: key → `{case → file path}` (variants; use `"/"` as default case)
   - **`many`**: key → `[file paths]` (list; files merged in order)
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

### IFS Implementations: Read Support

| IFS | Environment | `ReadAllBytes` | `ReadAllBytesAsync` | Calling thread |
|---|---|---|---|---|
| `DefaultFS` | .NET, Unity, Godot; only Windows, macOS and Linux, reading unpacked files, mainly for development | `File.ReadAllBytes` | `File.ReadAllBytesAsync`; returns an incomplete task and does not block the calling thread | Any thread |
| `UnityResourcesFS` | Unity | `Resources.Load` | `Resources.LoadAsync`, which needs Unity 6 or later; the package requires 6000.3, so it is always available. The result is delivered on the main thread | Main thread only |
| `UnityStreamingAssetsFS` | Unity | Not supported; throws `NotSupportedException` | `UnityWebRequest`; the result is delivered on the main thread | Main thread only |
| `UnityAddressablesFS` | Unity | Not supported; throws `NotSupportedException` | Addressables handles; the result is delivered on the main thread. Completes synchronously when the handles are already done | Main thread only |
| `UnityAddressablesGreedyFS` | Unity | Not supported; throws `NotSupportedException` | The first read of a file in a bundle caches the files of the whole bundle in memory. A later read of a file in the same bundle takes it from the cache and completes synchronously when the location handle is already done | Main thread only |
| `GodotFileAccessFS` (planned, not implemented yet) | Godot | `FileAccess.GetFileAsBytes` | Godot has no async file read API. The implementation runs the synchronous read with `Task.Run` | Any thread. The static `FileAccess` methods can be called from any thread: each call opens a new `FileAccess` object with its own native file handle. Never share one `FileAccess` object across threads. Do not mount a resource pack while files are being read, because the pack table is not locked |

### Planned Loading Flow (not implemented yet)

```
LoadAtlas / LoadAtlasAsync
│
├─ Check that the override directories exist                       [*]
│
├─ Choose how items are processed:
│    Is the main IFS or any override IFS MainThreadOnly?
│      Yes → Mode C (Caller: read at [*])
│      No  → Mode W (Worker: read and parse on a worker thread)
│
├─ Read and parse atlas.json, run the modifier, filter items       [*]
│    LoadAtlas uses ReadAllBytes. LoadAtlasAsync uses ReadAllBytes in Mode W
│    when workerThreadLoading or MainThreadParsing is on, and ReadAllBytesAsync otherwise.
│
├─ Scheduling loop                                                 [*]
│  │  Starts the items one by one in the chosen mode.
│  │  When MaxConcurrency items are in flight, waits for one to finish.
│  │  When an item fails, cancels the other items and starts no new ones.
│  │
│  │  Mode C: read the item's main files and override files        [*]
│  │          LoadAtlas uses ReadAllBytes; LoadAtlasAsync uses ReadAllBytesAsync
│  │    ├─ MainThreadParsing off: Task.Run
│  │    │    └─ Parse, merge the overrides, ApplyKeys
│  │    └─ MainThreadParsing on
│  │         └─ Parse, merge the overrides, ApplyKeys              [*]
│  │
│  │  Mode W:
│  │    ├─ MainThreadParsing off: Task.Run
│  │    │    ├─ Read the item's main files and override files
│  │    │    └─ Parse, merge the overrides, ApplyKeys
│  │    └─ MainThreadParsing on
│  │         ├─ Read the item's main files and override files      [*]
│  │         └─ Parse, merge the overrides, ApplyKeys              [*]
│  │
│  └─ Wait for all in-flight items; if any item failed, throw the first failure
│
├─ BindRefs                                                        [*]
└─ OnLoaded                                                        [*]
```

`[*]` means:

- `LoadAtlas`: on the calling thread.
- `LoadAtlasAsync`, when the caller has a `SynchronizationContext`: on that context, such as the Unity or Godot main thread. Otherwise, on the calling thread until the first asynchronous wait, then on thread pool threads; each later asynchronous wait may switch to another thread.

### Special Types

- **`XRef<V, T>`** — Cross-table reference; config ID stored, resolved in bind phase via `IRefBinder`
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

- `Newtonsoft.Json 13.0.3` — JSON serialization with custom converters (`XRefJsonConverter`, `DurationJsonConverter`)
- `xunit.v3 2.0.3` — Test framework
- C# 9.0, nullable enabled, implicit usings disabled

### Release & CI

- **`scripts/bump-version.sh`** — bumps `<Version>` in `Archmage.csproj` and `unity/.../package.json`, commits, and creates an annotated git tag; use `--yes` to skip interactive prompts
- **`scripts/release.sh`** — step-driven release automation; progress tracked in `release.json` by the `relstep` CLI (`go install github.com/shadowopera/archmage/tools/relstep@latest`); steps are declared in `STEP_LIST`
- **`CHANGELOG.md`** — updated only by the release workflow (`scripts/release.sh`); do not edit it during development
- **`scripts/reconcile-unity-meta.sh`** — checks Unity `.meta` file consistency
- **`.github/workflows/publish-nuget-github.yml`** — runs on `v*` tag push; verifies the tag version matches `Archmage.csproj` and `unity/.../package.json`, requires a `CHANGELOG.md` entry for the version, runs tests, pushes the NuGet package with the `NUGET_API_KEY` secret, and creates a GitHub release with the `.nupkg` and the Unity package `.tgz`
