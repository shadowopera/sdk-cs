---
title: 'IAtlas'
---

Namespace: Shadop.Archmage

Configuration collection managed by the Atlas loading system.

```csharp
public interface IAtlas
```

Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md)

**Remarks:**

Implementations store configuration items, resolve cross-table references via BindRefs,
 and perform post-load initialization in OnLoaded.

## Methods

### **SetDataVersion(VersionInfo)**

Stores VCS version metadata from the atlas.json.

```csharp
void SetDataVersion(VersionInfo v)
```

#### Parameters

`v` [VersionInfo](./shadop.archmage.versioninfo.md)<br>

### **AtlasItems()**

Gets all configuration items registered in this Atlas.

```csharp
Dictionary<string, AtlasItem> AtlasItems()
```

#### Returns

[Dictionary<String, AtlasItem>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **BindRefs()**

Resolves cross-table references after all items have been loaded.

```csharp
void BindRefs()
```

### **OnLoaded()**

Called after all items are loaded and references are bound.
 Perform validation, extension initialization, or throw to abort loading.

```csharp
void OnLoaded()
```
