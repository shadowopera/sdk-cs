---
title: 'AtlasLoadStage'
---

Namespace: Shadop.Archmage

Enumeration of stages during the loading of an individual Atlas item.

```csharp
public enum AtlasLoadStage
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [AtlasLoadStage](./shadop.archmage.atlasloadstage.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.ispanformattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

**Remarks:**

These stages are reported via [IProgress<T>](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1) during async loading
 to allow progress tracking and cancellation feedback.

## Fields

| Name | Value | Description |
| --- | --: | --- |
| StartProcessing | 0 | The loading process is about to process configuration file(s) for the item. |
| StartReading | 1 | The loading process is about to read the base configuration file from the filesystem. |
| StartParsing | 2 | The base configuration file content has been read and is about to be parsed as JSON. |
| StartReadingOverride | 3 | An override file has been found and is about to be read from the filesystem. |
| ApplyingOverride | 4 | An override file has been read and is being parsed and merged into the configuration. |
| Completed | 5 | The item has been fully loaded, parsed, and all overrides have been applied. |
