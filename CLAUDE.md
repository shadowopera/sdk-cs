# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the library
dotnet build src/Archmage/Archmage.csproj

# Build the Godot package (compile check only; the Godot code needs the engine to run)
dotnet build src/Archmage/Sdk/Godot/Archmage.Godot.csproj

# Run all tests
dotnet test tests/Archmage.Tests.csproj

# Run a single test by name
dotnet test tests/Archmage.Tests.csproj --filter "FullyQualifiedName~TestName"

# Regenerate golden files after intentional output changes
UPDATE_GOLDEN=1 dotnet test tests/Archmage.Tests.csproj

# Sync source to the Unity package, and test data to the Unity and Godot projects
scripts/rsync-engines.sh

# Run ArchmageDev PlayMode tests in batch mode (Unity Editor must be closed)
# --packed builds Addressables content and loads it from bundles ("Use Existing Build")
# Summary: unity/ArchmageDev/Logs/playmode-<assetdb|packed>-summary.txt
scripts/unity-test.sh [--no-sync] [--packed] [--filter <expr>]

# Run the Godot ArchmageDev tests; --exported runs them in a macOS program exported with the release template,
# which needs the Godot export templates
# Summary: godot/ArchmageDev/logs/godot-<path|exported>-summary.txt
scripts/godot-test.sh [--no-sync] [--exported] [--filter <regex>]

# Run .NET tests, then Unity PlayMode tests and Godot tests in both modes (args pass through to unity-test.sh only)
scripts/run-all-tests.sh

# Run PlatformProbe cases on a real platform (Unity Editor must be closed; cases: unity/ArchmageDev/Assets/PlatformProbe/PlatformProbe.cs)
scripts/webgl-probe.sh [--no-build] [<case>...]                                  # headless Chrome
scripts/android-probe.sh [--no-build] [--format apk|aab|aab-split] [<case>...]   # adb + logcat

# Start the Android emulator used by android-probe.sh (AVD archmage-probe: API 35, arm64-v8a)
~/Library/Android/sdk/emulator/emulator -avd archmage-probe -no-window -no-audio &

# Regenerate the API docs (sdk-cs, sdk-cs-unity, sdk-cs-godot) and sync them to ../docs (see "API Docs")
docs/update.sh

# Bump version
scripts/bump-version.sh [--yes] <version>  # e.g. 0.2.0

# Step-driven release workflow
scripts/release.sh [<version>]
```

## Architecture

**Archmage** is a C# configuration management SDK (namespace `Shadop.Archmage.Sdk`) for loading JSON-based game configs, targeting .NET (`net8.0`, `netstandard2.1`), Unity, and Godot 4.6 or later (.NET). The core library lives in `src/Archmage/`; the `unity/dev.shadop.archmage/Runtime/` directory is a mirror synced via `scripts/rsync-engines.sh`.

### Unity Package Structure

The Unity package (`unity/dev.shadop.archmage/`) uses three assemblies:

| Assembly | asmdef | Source | Notes |
|---|---|---|---|
| `Shadop.Archmage.Sdk` | `Runtime/Shadop.Archmage.Sdk.asmdef` | `src/Archmage/Sdk/*.cs` | Pure C#, `noEngineReferences: true` |
| `Shadop.Archmage.Sdk.Unity` | `Runtime/Unity/Shadop.Archmage.Sdk.Unity.asmdef` | `src/Archmage/Sdk/Unity/*.cs` | Unity engine adapters (`UnityResourcesFS`, `UnityStreamingAssetsFS`, `UnityAtlasLogger`) |
| `Shadop.Archmage.Sdk.Unity.Addressables` | `Runtime/Unity/Addressables/Shadop.Archmage.Sdk.Unity.Addressables.asmdef` | `src/Archmage/Sdk/Unity/Addressables/*.cs` | Addressables adapter; only compiled when `com.unity.addressables` is installed (`defineConstraints: ["UNITY_ADDRESSABLES"]`) |

`Shadop.Archmage.Sdk.Unity` and `Shadop.Archmage.Sdk.Unity.Addressables` both reference `Shadop.Archmage.Sdk`. The asmdef files are not synced by rsync — they live directly in the Unity package directory.

### Godot Package

The NuGet package `Shadop.Archmage.Godot` is built from `src/Archmage/Sdk/Godot/Archmage.Godot.csproj`, which sits next to its sources. It targets `net8.0`, references `GodotSharp` 4.6.0, and references `Archmage.csproj`, which becomes a dependency on `Shadop.Archmage` of the same version when packed. The sources (`GodotFileAccessFS`, `GodotAtlasLogger`, `GodotJsonSettingsFactory`, and the Vec, Rgba, MinMax and WeightedPool extensions) use the namespace `Shadop.Archmage.Sdk` and need no `#if`. `Archmage.csproj` excludes `Sdk/Godot/**`, and `scripts/rsync-engines.sh` does not sync it to Unity.

### Godot ArchmageDev Project

`godot/ArchmageDev/` is a Godot .NET project with the SDK integration tests and a demo (`scripts/ConfLoader.cs`, on the main scene `scenes/demo.tscn`).

- `ArchmageDev.csproj` references `Archmage.Godot.csproj` with `ProjectReference`, so changes in `src/` need no sync.
- `scripts/rsync-engines.sh` syncs `tests/testdata` to `configs/` and `tests/override` to `config_overrides/`. The `sdk-cs` recipe of `../whisper/JUSTFILE` generates `scripts/conf/`. `scripts/conf/AtlasExtension.cs` is not generated; keep it the same as `tests/Conf/AtlasExtension.cs`. The synced data and the generated code are committed.
- `addons/archmage/` is an editor plugin that loads the configs in the editor and shows config ID properties as dropdowns in the Inspector; `scenes/cfg_id_demo.tscn` shows them. The same recipe generates it with the `godot-editor` template. It writes `ArchmageEditorPlugin.cs` and `plugin.cfg` only when they do not exist, so edit those two here. To change the other files, edit `../whisper/archmage/structs/tpls/godot-editor.tpl`.
- `tests/TestRunner.cs` awaits each `[GodotTest]` method in turn on the Godot main thread. `scripts/godot-test.sh` builds the project before `--import`, because an import before the build reports the C# scripts as not compiling.
- From the project directory, the script passes `res://tests/test_runner.tscn` as the scene to run, so the demo does not start. The release template does not accept a scene path, so `--exported` exports the preset `macOS (tests)`, whose feature `archmage_tests` selects `run/main_scene.archmage_tests`, the runner.
- The export needs `ArchmageDev.sln`, `include_filter="*.json"` in the preset (Godot does not export JSON files by default) and `import_etc2_astc=true` in `project.godot` (required for arm64 and universal macOS builds). Godot exits with 0 when the .NET part of an export fails, so the script checks the export log for `ERROR`.

### Godot Editor Plugin

The code in `godot/ArchmageDev/addons/archmage/` runs in the Godot editor process.

- Put editor code in `#if TOOLS`. Exported programs do not contain `GodotSharpEditor`.
- The editor loads new C# code only after a build: the Build button, Play, or a `dotnet build` followed by switching to the editor window. Saving a `.cs` file does not build.
- After a build, the editor unloads the collectible `AssemblyLoadContext` of the game assembly and loads the new build. The reload clears static fields and does not call `_EnterTree` again, so the plugin loads the configs lazily from `CfgIdInspectorPlugin._CanHandle`.
- The unload fails ("Failed to unload assemblies"; the editor then runs the old code until it restarts) while a static cache outside the game assembly holds its types. `TypeDescriptor` is such a cache, and Newtonsoft.Json fills it during deserialization. Call `TypeDescriptorCleanup.Install()` before editor code deserializes configs. Games do not unload assemblies and are not affected.
- The Inspector shows only exported Variant types, so a config ID property is a `long` or `string` whose hint string names the CfgId type: `[Export(PropertyHint.None, "HeroCfgId")]`. For a list, use `long[]` with `[Export(PropertyHint.TypeString, "2/0:HeroCfgId")]`, or `string[]` with `"4/0:RaceCfgId"`. `Godot.Collections.Array<T>` drops the hint string.
- No automated test covers the plugin. To drive the editor from code, add a temporary file with a `[ModuleInitializer]` that uses `SceneTree` timers and `EditorInterface` (open a scene, `InspectObject`, find the `CfgIdEditorProperty` nodes, emit `OptionButton.ItemSelected`), and run `godot --headless --editor --path godot/ArchmageDev`. Change one dropdown per frame: when a probe changed two elements of one array in the same frame, only the last change was kept. A reload needs the GUI editor: start it, run `dotnet build`, then bring its window to the front, for example with `osascript`.

### API Docs

`docs/update.sh` generates the pages in `sdk-cs/`, `sdk-cs-unity/` and `sdk-cs-godot/` from the doc comments, so edit the doc comments, not the pages. The Unity and Godot pages are built from `docs/utils/api-unity/ArchmageUnityDocs.csproj` and `docs/utils/api-godot/ArchmageGodotDocs.csproj`. Each of them compiles the core sources together with the platform sources.

- Unity: the script uses the Unity Editor whose version is in `unity/ArchmageDev/ProjectSettings/ProjectVersion.txt` (set `UNITY_EDITOR` to override), and the Addressables assemblies in `unity/ArchmageDev/Library/ScriptAssemblies/`. After upgrading Unity, open ArchmageDev in the new Editor; nothing else needs to change. When the Unity sources start to use another UnityEngine module or `UNITY_*` symbol, add it to `ArchmageUnityDocs.csproj`.
- Godot: the script does not use the local Godot installation. It builds against the `GodotSharp` version in `Archmage.Godot.csproj`, so upgrading Godot locally needs no change.

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
| `IFS` | File system abstraction; `DefaultFS` wraps `System.IO`. `MainThreadOnly` chooses between Mode C and Mode W (see "Loading Flow") |
| `IAtlasLogger` | Logging; `DefaultLogger` writes to console |
| `AtlasOptions` | Builder for loader configuration (FS, logger, filters, overrides, concurrency, `MainThreadParsing`) |
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
| `GodotFileAccessFS` | Godot | `FileAccess.GetFileAsBytes` | Godot has no async file read API. The implementation runs the synchronous read with `Task.Run` | Any thread. The static `FileAccess` methods can be called from any thread: each call opens a new `FileAccess` object with its own native file handle. Never share one `FileAccess` object across threads. Do not mount a resource pack while files are being read, because the pack table is not locked |

### Loading Flow

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

- **`scripts/bump-version.sh`** — bumps `<Version>` in `Archmage.csproj`, `Archmage.Godot.csproj` and `unity/.../package.json`, commits, and creates an annotated git tag; use `--yes` to skip interactive prompts
- **`scripts/release.sh`** — step-driven release automation; progress tracked in `release.json` by the `relstep` CLI (`go install github.com/shadowopera/archmage/tools/relstep@latest`); steps are declared in `STEP_LIST`
- **`CHANGELOG.md`** — updated only by the release workflow (`scripts/release.sh`); do not edit it during development
- **`scripts/reconcile-unity-meta.sh`** — checks Unity `.meta` file consistency
- **`.github/workflows/publish-nuget-github.yml`** — runs on `v*` tag push; verifies the tag version matches `Archmage.csproj`, `Archmage.Godot.csproj` and `unity/.../package.json`, requires a `CHANGELOG.md` entry for the version, runs tests, pushes the `Shadop.Archmage` and `Shadop.Archmage.Godot` NuGet packages with the `NUGET_API_KEY` secret, and creates a GitHub release with both `.nupkg` files and the Unity package `.tgz`
