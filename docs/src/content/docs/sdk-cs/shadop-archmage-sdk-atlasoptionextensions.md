---
title: 'AtlasOptionExtensions'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for fluent configuration of AtlasOptions using the builder pattern.

```csharp
public static class AtlasOptionExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AtlasOptionExtensions](../shadop-archmage-sdk-atlasoptionextensions/)<br>

**Remarks:**

All methods return the AtlasOptions instance to enable method chaining.

## Methods

### **WithLogger(AtlasOptions, IAtlasLogger)**

Sets custom logger (defaults to console).

```csharp
public static AtlasOptions WithLogger(AtlasOptions opts, IAtlasLogger logger)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`logger` [IAtlasLogger](../shadop-archmage-sdk-iatlaslogger/)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if logger is null.

### **WithFS(AtlasOptions, IFS)**

Sets custom filesystem (in-memory, embedded, virtual, etc. — does not affect override sources).

```csharp
public static AtlasOptions WithFS(AtlasOptions opts, IFS fs)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`fs` [IFS](../shadop-archmage-sdk-ifs/)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if fs is null.

### **WithAtlasModifier(AtlasOptions, Action<AtlasJson>)**

Registers callback to modify atlas.json after loading.

```csharp
public static AtlasOptions WithAtlasModifier(AtlasOptions opts, Action<AtlasJson> modifier)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`modifier` [Action<AtlasJson>](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if modifier is null.

### **WithWhitelist(AtlasOptions, IEnumerable<String>)**

Specifies whitelist (if set, blacklist ignored; loads only whitelisted items).

```csharp
public static AtlasOptions WithWhitelist(AtlasOptions opts, IEnumerable<string> whitelist)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`whitelist` [IEnumerable<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if whitelist is null.

### **WithBlacklist(AtlasOptions, IEnumerable<String>)**

Specifies blacklist (ignored if whitelist present; skips blacklisted items).

```csharp
public static AtlasOptions WithBlacklist(AtlasOptions opts, IEnumerable<string> blacklist)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`blacklist` [IEnumerable<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if blacklist is null.

### **WithOverrideRoot(AtlasOptions, String)**

Adds directory as override source (processed in order; each can override previous).

```csharp
public static AtlasOptions WithOverrideRoot(AtlasOptions opts, string rootPath)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`rootPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Thrown if rootPath is null or whitespace.

### **WithOverrideFS(AtlasOptions, IFS, String)**

Adds custom filesystem as override source (embedded, network, in-memory, etc.).
 When rootPath is specified, override files are resolved relative to that path within the filesystem.

```csharp
public static AtlasOptions WithOverrideFS(AtlasOptions opts, IFS fs, string rootPath)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`fs` [IFS](../shadop-archmage-sdk-ifs/)<br>

`rootPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if fs is null.

### **WithLoadStrategy(AtlasOptions, AtlasLoadStrategy)**

Sets custom load strategy for parallel/cached/conditional loading (default is sequential).

```csharp
public static AtlasOptions WithLoadStrategy(AtlasOptions opts, AtlasLoadStrategy strategy)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`strategy` AtlasLoadStrategy<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if strategy is null.

### **WithAsyncLoadStrategy(AtlasOptions, AtlasAsyncLoadStrategy)**

Sets custom async load strategy for parallel/cached/conditional loading (default is sequential).

```csharp
public static AtlasOptions WithAsyncLoadStrategy(AtlasOptions opts, AtlasAsyncLoadStrategy strategy)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`strategy` AtlasAsyncLoadStrategy<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if strategy is null.

### **WithJsonSettings(AtlasOptions, JsonSerializerSettings)**

Sets custom JSON settings.

```csharp
public static AtlasOptions WithJsonSettings(AtlasOptions opts, JsonSerializerSettings settings)
```

#### Parameters

`opts` [AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

`settings` JsonSerializerSettings<br>

#### Returns

[AtlasOptions](../shadop-archmage-sdk-atlasoptions/)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if settings is null.
