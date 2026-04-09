---
title: 'AtlasLoadEvent'
---

Namespace: Shadop.Archmage.Sdk

Progress event data reported during asynchronous Atlas item loading.

```csharp
public struct AtlasLoadEvent
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [AtlasLoadEvent](../shadop-archmage-sdk-atlasloadevent/)<br>

**Remarks:**

AtlasLoadEvent instances are reported to an [IProgress<T>](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1)
 implementation passed to [Archmage.LoadAtlasAsync(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>, CancellationToken)](../shadop-archmage-sdk-archmage/#loadatlasasyncstring-string-iatlas-atlasoptions-iprogressatlasloadevent-cancellationtoken).
 This allows consumers to track loading progress and provide feedback to the user.

The Elapsed property can be used to implement timeouts or progress visualization.

## Properties

### **Key**

```csharp
public string Key { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Stage**

```csharp
public AtlasLoadStage Stage { get; }
```

#### Property Value

[AtlasLoadStage](../shadop-archmage-sdk-atlasloadstage/)<br>

### **FilePath**

Set during StartReading, StartParsing, StartReadingOverride, and ApplyingOverride; null for StartProcessing and Completed.

```csharp
public string FilePath { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Elapsed**

```csharp
public TimeSpan Elapsed { get; }
```

#### Property Value

[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

### **Total**

The total number of atlas items to load. Only meaningful for [AtlasLoadStage.ItemsQueued](../shadop-archmage-sdk-atlasloadstage/#itemsqueued).

```csharp
public int Total { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **AtlasLoadEvent(String, AtlasLoadStage, String, TimeSpan, Int32)**

```csharp
AtlasLoadEvent(string key, AtlasLoadStage stage, string filePath, TimeSpan elapsed, int total)
```

#### Parameters

`key` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`stage` [AtlasLoadStage](../shadop-archmage-sdk-atlasloadstage/)<br>

`filePath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`elapsed` [TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

`total` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
