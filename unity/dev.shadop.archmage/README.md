# Archmage

Atlas configuration system for loading and managing configurations from JSON files
with support for i18n, references, durations, and layered overrides.

## Requirements

- Unity 6000.3 or later
- com.unity.nuget.newtonsoft-json 3.2.2

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

## Features

- **Atlas** — Load, merge and manage structured configuration data from JSON files
- **Duration** — Time duration type with compact shard encoding
- **Vec2 / Vec3 / Vec4** — Lightweight vector types with JSON support
- **Tup1–Tup7** — Mixed-type tuple types for structured data
- **XRef** — Deferred reference resolution across atlas items
- **I18n** — Internationalization support

## License

Apache 2.0. See [LICENSE.md](LICENSE.md) for details.
