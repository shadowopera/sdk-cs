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

# Regenerate the API docs (sdk-cs, sdk-cs-unity, sdk-cs-unity-editor, sdk-cs-godot) and sync them to ../docs (see "API Docs")
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
| `Shadop.Archmage.Sdk.Unity` | `Runtime/Unity/Shadop.Archmage.Sdk.Unity.asmdef` | `src/Archmage/Sdk/Unity/*.cs` | Unity engine adapters |
| `Shadop.Archmage.Sdk.Unity.Addressables` | `Runtime/Unity/Addressables/Shadop.Archmage.Sdk.Unity.Addressables.asmdef` | `src/Archmage/Sdk/Unity/Addressables/*.cs` | Addressables adapter; only compiled when `com.unity.addressables` is installed (`defineConstraints: ["UNITY_ADDRESSABLES"]`) |

The asmdef files are not synced by rsync — they live directly in the Unity package directory.

### Godot Package

The NuGet package `Shadop.Archmage.Godot` is built from `src/Archmage/Sdk/Godot/Archmage.Godot.csproj`, which sits next to its sources. The sources use the namespace `Shadop.Archmage.Sdk` and need no `#if`. `Archmage.csproj` excludes `Sdk/Godot/**`, and `scripts/rsync-engines.sh` does not sync it to Unity.

### Godot ArchmageDev Project

`godot/ArchmageDev/` is a Godot .NET project with the SDK integration tests and a demo (`scripts/ConfLoader.cs`, on the main scene `scenes/demo.tscn`).

- `ArchmageDev.csproj` references `Archmage.Godot.csproj` with `ProjectReference`, so changes in `src/` need no sync.
- `scripts/rsync-engines.sh` syncs `tests/testdata` to `configs/` and `tests/override` to `config_overrides/`. The `sdk-cs` recipe of `../whisper/JUSTFILE` generates `scripts/conf/`. `scripts/conf/AtlasExtension.cs` is not generated; keep it the same as `tests/Conf/AtlasExtension.cs`. The synced data and the generated code are committed.
- `addons/archmage/` is an editor plugin that loads the configs in the editor and shows config ID properties as dropdowns in the Inspector; the root node of `scenes/demo.tscn` shows them. The same recipe generates it with the `godot-editor` template. It writes `ArchmageEditorPlugin.cs` and `plugin.cfg` only when they do not exist, so edit those two here. To change the other files, edit `../whisper/archmage/structs/tpls/godot-editor.tpl`.
- `tests/TestRunner.cs` awaits each `[GodotTest]` method in turn on the Godot main thread. `scripts/godot-test.sh` builds the project before `--import`, because an import before the build reports the C# scripts as not compiling.
- From the project directory, the script passes `res://tests/test_runner.tscn` as the scene to run, so the demo does not start. The release template does not accept a scene path, so `--exported` exports the preset `macOS (tests)`, whose feature `archmage_tests` selects `run/main_scene.archmage_tests`, the runner.
- The export needs `ArchmageDev.sln`, `include_filter="*.json"` in the preset (Godot does not export JSON files by default) and `import_etc2_astc=true` in `project.godot` (required for arm64 and universal macOS builds). Godot exits with 0 when the .NET part of an export fails, so the script checks the export log for `ERROR`.

### Godot Editor Plugin

The code in `godot/ArchmageDev/addons/archmage/` runs in the Godot editor process.

- Put editor code in `#if TOOLS`. Exported programs do not contain `GodotSharpEditor`.
- The editor loads new C# code only after a build: the Build button, Play, or a `dotnet build` followed by switching to the editor window. Saving a `.cs` file does not build.
- After a build, the editor unloads the collectible `AssemblyLoadContext` of the game assembly and loads the new build. The reload clears static fields and does not call `_EnterTree` again, so the plugin loads the configs lazily from `CfgIdInspectorPlugin._CanHandle`.
- The unload fails ("Failed to unload assemblies"; the editor then runs the old code until it restarts) while a static cache outside the game assembly holds its types. `TypeDescriptor` is such a cache, and Newtonsoft.Json fills it during deserialization. Call `TypeDescriptorCleanup.Install()` before editor code deserializes configs. Games do not unload assemblies and are not affected.
- Godot cannot export a CfgId type, so a config ID is exported as its `Value` type, and the plugin recognizes it by the hint string. Game code uses the constants of `CfgIdPropHint` and `CfgIdPropType`, so `DefaultCfgIdChoices.cs` defines them outside `#if TOOLS`.
- No automated test covers the plugin. To drive the editor from code, add a temporary file with a `[ModuleInitializer]` that uses `SceneTree` timers and `EditorInterface` (open a scene, `InspectObject`, find the `CfgIdEditorProperty` nodes, emit `OptionButton.ItemSelected`), and run `godot --headless --editor --path godot/ArchmageDev`. Change one dropdown per frame: when a probe changed two elements of one array in the same frame, only the last change was kept. A reload needs the GUI editor: start it, run `dotnet build`, then bring its window to the front, for example with `osascript`.

### API Docs

`docs/update.sh` generates the pages in `sdk-cs/`, `sdk-cs-unity/`, `sdk-cs-unity-editor/` and `sdk-cs-godot/` from the doc comments, so edit the doc comments, not the pages. The Unity and Unity Editor pages are built from `docs/utils/api-unity/ArchmageUnityDocs.csproj`, and the Godot pages from `docs/utils/api-godot/ArchmageGodotDocs.csproj`. The pages in `gen-cs-editor/` explain how to use the code that the `unity-editor` and `godot-editor` templates generate; they are written by hand.

- Unity: the script uses the Unity Editor whose version is in `unity/ArchmageDev/ProjectSettings/ProjectVersion.txt` (set `UNITY_EDITOR` to override), and the Addressables assemblies in `unity/ArchmageDev/Library/ScriptAssemblies/`. After upgrading Unity, open ArchmageDev in the new Editor; nothing else needs to change. When the Unity or Unity Editor sources start to use another UnityEngine or UnityEditor module or `UNITY_*` symbol, add it to `ArchmageUnityDocs.csproj`.
- xmldoc2md writes nothing for `<typeparamref>` and for some `<see cref>` to methods, which leaves a gap in the sentence. Write the name in `<c>` instead, such as `<c>TId</c>`.
- Godot: the script does not use the local Godot installation. It builds against the `GodotSharp` version in `Archmage.Godot.csproj`, so upgrading Godot locally needs no change.

### README and CHANGELOG Copies

Edit `README.md` and `CHANGELOG.md` in the repository root, not their copies. The next sync overwrites the copies.

- `scripts/rsync-engines.sh` copies `README.md` to `unity/dev.shadop.archmage/README.md`, and `docs/update.sh` copies it to `docs/src/content/docs/overview-cs/sdk-cs.mdx`.
- `scripts/rsync-engines.sh` copies `CHANGELOG.md` to `unity/dev.shadop.archmage/CHANGELOG.md`, and `docs/update.sh` copies it to `docs/src/content/docs/overview-cs/CHANGELOG.md`.

### atlas.json

`atlas.json` maps each config key in one of three ways:

- **`unique`**: key → file path
- **`variant`**: key → `{case → file path}`; `"/"` is the default case
- **`many`**: key → `[file paths]`; the files are merged in order

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

### Generated Config Code

`tests/Conf/` is a sample of the config code that users generate with archmage; the `sdk-cs` recipe of `../whisper/JUSTFILE` generates it with the `json-cs` template. To change a file marked "DO NOT EDIT", edit `../whisper/archmage/structs/tpls/json-cs.tpl`. Files marked "Safe to edit", such as `AtlasExtension.cs`, are not regenerated.

### Language

C# 9.0, nullable enabled, implicit usings disabled.

### Release & CI

- **`scripts/bump-version.sh`** — bumps `<Version>` in `Archmage.csproj`, `Archmage.Godot.csproj` and `unity/.../package.json`, commits, and creates an annotated git tag; use `--yes` to skip interactive prompts
- **`scripts/release.sh`** — step-driven release automation; progress tracked in `release.json` by the `relstep` CLI (`go install github.com/shadowopera/archmage/tools/relstep@latest`); steps are declared in `STEP_LIST`
- **`CHANGELOG.md`** — updated only by the release workflow (`scripts/release.sh`); do not edit it during development
- **`scripts/reconcile-unity-meta.sh`** — checks Unity `.meta` file consistency
- **`.github/workflows/publish-nuget-github.yml`** — runs on `v*` tag push; verifies the tag version matches `Archmage.csproj`, `Archmage.Godot.csproj` and `unity/.../package.json`, requires a `CHANGELOG.md` entry for the version, runs tests, pushes the `Shadop.Archmage` and `Shadop.Archmage.Godot` NuGet packages with the `NUGET_API_KEY` secret, and creates a GitHub release with both `.nupkg` files and the Unity package `.tgz`
