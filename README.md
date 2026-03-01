# Archmage

Atlas configuration system for loading and managing configurations from JSON files
with support for i18n, references, durations, and layered overrides.

## Installation

### Unity (via OpenUPM)

```bash
openupm add dev.shadop.archmage
```

Or add to `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": ["dev.shadop"]
    }
  ],
  "dependencies": {
    "dev.shadop.archmage": "0.1.0"
  }
}
```

### .NET (via NuGet)

```bash
dotnet add package Shadop.Archmage
```

## Project Structure

```
sdk-cs/
├── src/Archmage/          # C# source (canonical)
├── unity/                 # Unity package (OpenUPM)
│   ├── package.json
│   ├── Runtime/           # Synced from src/
│   └── ...
├── scripts/
│   └── sync-unity.sh      # src/ → unity/Runtime/ sync
└── ...
```

## Development

### Build

```bash
dotnet build src/Archmage/Archmage.csproj
```

### Sync to Unity

```bash
bash scripts/sync-unity.sh
```

## License

Apache 2.0. See [LICENSE](LICENSE) for details.
