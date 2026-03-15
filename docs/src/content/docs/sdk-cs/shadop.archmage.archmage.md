---
title: 'Archmage'
---

Namespace: Shadop.Archmage

Provides utility functions.

```csharp
public static class Archmage
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Archmage](./shadop.archmage.archmage.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Methods

### **DumpAtlas(IAtlas, String, JsonSerializerSettings)**

Exports Atlas items to {key}.json files (only Ready items; pretty-printed).

```csharp
public static void DumpAtlas(IAtlas atlas, string outputDir, JsonSerializerSettings settings)
```

#### Parameters

`atlas` [IAtlas](./shadop.archmage.iatlas.md)<br>

`outputDir` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`settings` JsonSerializerSettings<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if atlas or outputDir is null.

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>
Thrown if outputDir is empty or whitespace.

**Remarks:**

Useful for debugging, testing (golden files), and human-readable export.
 Output uses custom converters for Duration/XRef types.

### **CreateJsonSerializerSettings()**

Creates JSON settings for Atlas export (indented, include nulls/defaults, custom converters).

```csharp
public static JsonSerializerSettings CreateJsonSerializerSettings()
```

#### Returns

JsonSerializerSettings<br>

### **LoadAtlas(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>)**

Loads an Atlas synchronously from the specified index file and root directory.

```csharp
public static void LoadAtlas(string atlasFile, string cfgRoot, IAtlas atlas, AtlasOptions options, IProgress<AtlasLoadEvent> progress)
```

#### Parameters

`atlasFile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to atlas.json containing mapping definitions.

`cfgRoot` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Root directory where configuration JSON files are located.

`atlas` [IAtlas](./shadop.archmage.iatlas.md)<br>
The Atlas implementation to populate with loaded items.

`options` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>
Optional loading configuration. If null, default options are used.

`progress` [IProgress<AtlasLoadEvent>](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1)<br>
Optional callback for receiving progress reports.

#### Exceptions

[ArchmageException](./shadop.archmage.archmageexception.md)<br>
Thrown if loading fails at any stage.

**Remarks:**

This method performs the following steps:

1. - Reads and parses atlas.json
2. - Applies any registered modifier callbacks to the atlas data
3. - Loads each configuration item by reading files, deserializing, and merging overrides
4. - Calls BindRefs to resolve cross-table references
5. - Calls OnLoaded on the Atlas for post-load initialization

If any step fails, an ArchmageException is raised and loading is aborted.
 Alternatively, exceptions can be thrown from IAtlas.OnLoaded() to abort loading.

### **LoadAtlasAsync(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>, CancellationToken)**

Loads an Atlas asynchronously with progress reporting and cancellation support.

```csharp
public static Task LoadAtlasAsync(string atlasFile, string cfgRoot, IAtlas atlas, AtlasOptions options, IProgress<AtlasLoadEvent> progress, CancellationToken cancellationToken)
```

#### Parameters

`atlasFile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to atlas.json containing mapping definitions.

`cfgRoot` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Root directory where configuration JSON files are located.

`atlas` [IAtlas](./shadop.archmage.iatlas.md)<br>
The Atlas implementation to populate with loaded items.

`options` [AtlasOptions](./shadop.archmage.atlasoptions.md)<br>
Optional loading configuration. If null, default options are used.

`progress` [IProgress<AtlasLoadEvent>](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1)<br>
Optional callback for receiving progress reports.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
Token to request cancellation of the loading operation.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A Task representing the asynchronous loading operation.

#### Exceptions

[ArchmageException](./shadop.archmage.archmageexception.md)<br>
Thrown if loading fails at any stage.

[OperationCanceledException](https://docs.microsoft.com/en-us/dotnet/api/system.operationcanceledexception)<br>
Thrown if cancellation is requested.

**Remarks:**

This method performs the same operations as LoadAtlas but asynchronously, providing non-blocking I/O.
 Progress events are reported via the IProgress interface to allow UI updates and status tracking.
 Loading can be cancelled via the CancellationToken.

### **ShardDuration(Duration)**

Encodes Duration to compact array (chooses smallest representation based on value; zero → null).

```csharp
public static Int64[] ShardDuration(Duration duration)
```

#### Parameters

`duration` [Duration](./shadop.archmage.duration.md)<br>

#### Returns

[Int64[]](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **ParseDurationShards(Int64[])**

Decodes shard array to Duration (null/empty → Duration.Zero).

```csharp
public static Duration ParseDurationShards(Int64[] shards)
```

#### Parameters

`shards` [Int64[]](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

#### Returns

[Duration](./shadop.archmage.duration.md)<br>

#### Exceptions

[ArchmageException](./shadop.archmage.archmageexception.md)<br>
Thrown if invalid format or unknown type.

### **MakeTuple<T0>(T0)**

```csharp
public static Tup1<T0> MakeTuple<T0>(T0 item0)
```

#### Type Parameters

`T0`<br>

#### Parameters

`item0` T0<br>

#### Returns

Tup1<T0><br>

### **MakeTuple<T0, T1>(T0, T1)**

```csharp
public static Tup2<T0, T1> MakeTuple<T0, T1>(T0 item0, T1 item1)
```

#### Type Parameters

`T0`<br>

`T1`<br>

#### Parameters

`item0` T0<br>

`item1` T1<br>

#### Returns

Tup2<T0, T1><br>

### **MakeTuple<T0, T1, T2>(T0, T1, T2)**

```csharp
public static Tup3<T0, T1, T2> MakeTuple<T0, T1, T2>(T0 item0, T1 item1, T2 item2)
```

#### Type Parameters

`T0`<br>

`T1`<br>

`T2`<br>

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

#### Returns

Tup3<T0, T1, T2><br>

### **MakeTuple<T0, T1, T2, T3>(T0, T1, T2, T3)**

```csharp
public static Tup4<T0, T1, T2, T3> MakeTuple<T0, T1, T2, T3>(T0 item0, T1 item1, T2 item2, T3 item3)
```

#### Type Parameters

`T0`<br>

`T1`<br>

`T2`<br>

`T3`<br>

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

`item3` T3<br>

#### Returns

Tup4<T0, T1, T2, T3><br>

### **MakeTuple<T0, T1, T2, T3, T4>(T0, T1, T2, T3, T4)**

```csharp
public static Tup5<T0, T1, T2, T3, T4> MakeTuple<T0, T1, T2, T3, T4>(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
```

#### Type Parameters

`T0`<br>

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

`item3` T3<br>

`item4` T4<br>

#### Returns

Tup5<T0, T1, T2, T3, T4><br>

### **MakeTuple<T0, T1, T2, T3, T4, T5>(T0, T1, T2, T3, T4, T5)**

```csharp
public static Tup6<T0, T1, T2, T3, T4, T5> MakeTuple<T0, T1, T2, T3, T4, T5>(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
```

#### Type Parameters

`T0`<br>

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

`item3` T3<br>

`item4` T4<br>

`item5` T5<br>

#### Returns

Tup6<T0, T1, T2, T3, T4, T5><br>

### **MakeTuple<T0, T1, T2, T3, T4, T5, T6>(T0, T1, T2, T3, T4, T5, T6)**

```csharp
public static Tup7<T0, T1, T2, T3, T4, T5, T6> MakeTuple<T0, T1, T2, T3, T4, T5, T6>(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
```

#### Type Parameters

`T0`<br>

`T1`<br>

`T2`<br>

`T3`<br>

`T4`<br>

`T5`<br>

`T6`<br>

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

`item3` T3<br>

`item4` T4<br>

`item5` T5<br>

`item6` T6<br>

#### Returns

Tup7<T0, T1, T2, T3, T4, T5, T6><br>
