---
title: 'AtlasLoadStage'
---

Namespace: Shadop.Archmage.Sdk

Enumeration of stages during Atlas loading.

```csharp
public enum AtlasLoadStage
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [AtlasLoadStage](../shadop-archmage-sdk-atlasloadstage/)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

These stages are reported via [IProgress<T>](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1) during async loading
 to allow progress tracking and cancellation feedback.

`ItemsQueued` is reported once, before loading any items.
 All other stages are reported per item.

## Fields

| Name | Value | Description |
| --- | --: | --- |
| ItemsQueued | 0 | Atlas loading is about to begin. [AtlasLoadEvent.Total](../shadop-archmage-sdk-atlasloadevent/#total) indicates the number of items to load. |
| StartProcessing | 1 | The loading process is about to process configuration file(s) for the item. |
| StartReading | 2 | The loading process is about to read the base configuration file from the filesystem. |
| StartParsing | 3 | The base configuration file content has been read and is about to be parsed as JSON. |
| StartReadingOverride | 4 | An override file has been found and is about to be read from the filesystem. |
| ApplyingOverride | 5 | An override file has been read and is being parsed and merged into the configuration. |
| Completed | 6 | The item has been fully loaded, parsed, and all overrides have been applied. |
