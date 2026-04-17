---
title: 'ConfigAtlas'
description: 'Holds all game configuration tables and provides cross-table reference binding.'
---

Holds all game configuration tables and provides cross-table reference binding.

```csharp
public class ConfigAtlas : IAtlas
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ConfigAtlas](.)<br>
Implements [IAtlas](../../sdk-cs/shadop-archmage-sdk-iatlas/)

## Fields

### **Instance**

The active ConfigAtlas. It must be set before calling any XxxCfgId.Cfg.

```csharp
public static ConfigAtlas Instance
```

#### Field Value

[ConfigAtlas](.)<br>

## Properties

### **Extension**

```csharp
public AtlasExtension Extension { get; private set; }
```

#### Property Value

AtlasExtension<br>

### **DataVersion**

The version info of the config repo at export time.

```csharp
public VersionInfo? DataVersion { get; private set; }
```

#### Property Value

[VersionInfo](../../sdk-cs/shadop-archmage-sdk-versioninfo/)<br>

### **CodeVersion**

The version info of the config repo at codegen time.

```csharp
public static VersionInfo CodeVersion { get; }
```

#### Property Value

[VersionInfo](../../sdk-cs/shadop-archmage-sdk-versioninfo/)<br>

## Constructors

### **ConfigAtlas()**

```csharp
public ConfigAtlas()
```

## Methods

### **SetDataVersion(VersionInfo)**

For internal use only.

```csharp
public void SetDataVersion(VersionInfo? v)
```

#### Parameters

`v` [VersionInfo](../../sdk-cs/shadop-archmage-sdk-versioninfo/)<br>

### **BindRefs()**

For internal use only.

```csharp
public void BindRefs()
```

### **AtlasItems()**

Returns the internal map of config-name to [AtlasItem](../../sdk-cs/shadop-archmage-sdk-atlasitem/).

```csharp
public Dictionary<string, AtlasItem> AtlasItems()
```

#### Returns

[Dictionary<String, AtlasItem>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **OnLoaded()**

Called after all config data has been loaded and refs bound. Delegates to AtlasExtension.OnLoaded. For internal use only.

```csharp
public void OnLoaded()
```
