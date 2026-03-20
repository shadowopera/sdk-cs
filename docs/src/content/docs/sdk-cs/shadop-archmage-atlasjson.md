---
title: 'AtlasJson'
---

Namespace: Shadop.Archmage

Represents the structure of an Atlas index JSON file (atlas.json).

```csharp
public class AtlasJson
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AtlasJson](../shadop-archmage-atlasjson/)<br>

**Remarks:**

The atlas index file contains VCS metadata and three mapping strategies that define how configuration item keys map to JSON files.

## Properties

### **Version**

VCS metadata (workspace, commit hash, branch, timestamp). May be null if unavailable during build.

```csharp
public VersionInfo Version { get; set; }
```

#### Property Value

[VersionInfo](../shadop-archmage-versioninfo/)<br>

### **Unique**

One-to-one mapping (key → file path).

```csharp
public Dictionary<string, string> Unique { get; set; }
```

#### Property Value

[Dictionary<String, String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **Single**

One-to-many conditional mapping (key → {case → file path}). Use "/" as the default case.

```csharp
public Dictionary<string, Dictionary<string, string>> Single { get; set; }
```

#### Property Value

[Dictionary<String, Dictionary<String, String>>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **Multiple**

One-to-many list mapping (key → [file paths]). Files merged in order.

```csharp
public Dictionary<string, List<string>> Multiple { get; set; }
```

#### Property Value

[Dictionary<String, List<String>>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

## Constructors

### **AtlasJson()**

```csharp
public AtlasJson()
```
