# Archmage

Atlas configuration system for loading and managing configurations from JSON files
with support for i18n, references, durations, and layered overrides.

## Requirements

- Unity 6000.3 or later
- `System.Text.Json` — this package depends on System.Text.Json but does **not** bundle it, to avoid DLL conflicts with other packages in your project. See [Dependencies](#dependencies) below.

## Installation

### Via OpenUPM (Recommended)

```bash
openupm add dev.shadop.archmage
```

### Via Unity Package Manager

Add the following to your `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "dev.shadop"
      ]
    }
  ],
  "dependencies": {
    "dev.shadop.archmage": "0.1.0"
  }
}
```

## Dependencies

This package requires `System.Text.Json` (>= 8.0.0). Choose one of the following methods:

### Option A: Via NuGetForUnity (Recommended)

1. Install [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)
2. Open **NuGet > Manage NuGet Packages** in Unity
3. Search for `System.Text.Json` and install it

### Option B: Manual DLL

1. Download `System.Text.Json` from [NuGet.org](https://www.nuget.org/packages/System.Text.Json)
2. Extract the `.nupkg` file (rename to `.zip`)
3. Copy `lib/netstandard2.0/System.Text.Json.dll` into your project's `Assets/Plugins/` folder

## Features

- **Atlas** — Load, merge and manage structured configuration data from JSON files
- **Duration** — Time duration type with compact shard encoding
- **Vec2 / Vec3 / Vec4** — Lightweight vector types with JSON support
- **Tup1–Tup7** — Mixed-type tuple types for structured data
- **Ref** — Deferred reference resolution across atlas items
- **I18n** — Internationalization support

## License

Apache 2.0. See [LICENSE.md](LICENSE.md) for details.
