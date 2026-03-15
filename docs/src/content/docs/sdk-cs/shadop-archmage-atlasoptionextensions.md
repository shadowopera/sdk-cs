---
title: 'AtlasOptionExtensions'
---

Namespace: Shadop.Archmage

Extension methods for fluent configuration of AtlasOptions using the builder pattern.

```csharp
public static class AtlasOptionExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AtlasOptionExtensions](./shadop.archmage.atlasoptionextensions.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md), [ExtensionAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.extensionattribute)

**Remarks:**

All methods return the AtlasOptions instance to enable method chaining.

## Methods

### **WithLogger(AtlasOptions, IAtlasLogger)**

Sets custom logger (defaults to console).

```csharp
public static AtlasOptions WithLogger(AtlasOptions opts, IAtlasLogger logger)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`logger` [IAtlasLogger](./shadop.archmage.iatlaslogger.md)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if logger is null.

### **WithFS(AtlasOptions, IFS)**

Sets custom filesystem (in-memory, embedded, virtual, etc. — does not affect override sources).

```csharp
public static AtlasOptions WithFS(AtlasOptions opts, IFS fs)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`fs` [IFS](./shadop.archmage.ifs.md)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if fs is null.

### **WithAtlasModifier(AtlasOptions, Action<AtlasJson>)**

Registers callback to modify atlas.json after loading.

```csharp
public static AtlasOptions WithAtlasModifier(AtlasOptions opts, Action<AtlasJson> modifier)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`modifier` [Action<AtlasJson>](https://docs.microsoft.com/en-us/dotnet/api/system.action-1)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if modifier is null.

### **WithWhitelist(AtlasOptions, IEnumerable<String>)**

Specifies whitelist (if set, blacklist ignored; loads only whitelisted items).

```csharp
public static AtlasOptions WithWhitelist(AtlasOptions opts, IEnumerable<string> whitelist)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`whitelist` [IEnumerable<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if whitelist is null.

### **WithBlacklist(AtlasOptions, IEnumerable<String>)**

Specifies blacklist (ignored if whitelist present; skips blacklisted items).

```csharp
public static AtlasOptions WithBlacklist(AtlasOptions opts, IEnumerable<string> blacklist)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`blacklist` [IEnumerable<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if blacklist is null.

### **WithOverrideRoot(AtlasOptions, String)**

Adds directory as override source (processed in order; each can override previous).

```csharp
public static AtlasOptions WithOverrideRoot(AtlasOptions opts, string rootPath)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`rootPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Thrown if rootPath is null or whitespace.

### **WithOverrideFS(AtlasOptions, IFS)**

Adds custom filesystem as override source (embedded, network, in-memory, etc.).

```csharp
public static AtlasOptions WithOverrideFS(AtlasOptions opts, IFS fs)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`fs` [IFS](./shadop.archmage.ifs.md)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if fs is null.

### **WithLoadStrategy(AtlasOptions, AtlasLoadStrategy)**

Sets custom load strategy for parallel/cached/conditional loading (default is sequential).

```csharp
public static AtlasOptions WithLoadStrategy(AtlasOptions opts, AtlasLoadStrategy strategy)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`strategy` [AtlasLoadStrategy](./shadop.archmage.atlasloadstrategy.md)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if strategy is null.

### **WithAsyncLoadStrategy(AtlasOptions, AtlasAsyncLoadStrategy)**

Sets custom async load strategy for parallel/cached/conditional loading (default is sequential).

```csharp
public static AtlasOptions WithAsyncLoadStrategy(AtlasOptions opts, AtlasAsyncLoadStrategy strategy)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`strategy` [AtlasAsyncLoadStrategy](./shadop.archmage.atlasasyncloadstrategy.md)<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if strategy is null.

### **WithJsonSettings(AtlasOptions, JsonSerializerSettings)**

Sets custom JSON settings.

```csharp
public static AtlasOptions WithJsonSettings(AtlasOptions opts, JsonSerializerSettings settings)
```

#### Parameters

`opts` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

`settings` JsonSerializerSettings<br>

#### Returns

[AtlasOptions](./shadop.archmage.atlasoptions.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if settings is null.
