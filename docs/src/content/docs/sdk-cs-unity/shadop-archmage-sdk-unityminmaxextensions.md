---
title: 'UnityMinMaxExtensions'
description: 'Extension methods for sampling a uniform random value from a MinMax<T> range using Unity''s global Random.'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for [MinMax](../../sdk-cs/shadop-archmage-sdk-minmax-1/).

```csharp
public static class UnityMinMaxExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityMinMaxExtensions](.)

## Supported Types

To maximize performance and avoid garbage collection (GC) allocations in Unity, these extensions provide explicit, hardcoded overloads for standard numeric types.

The underlying generic type `T` can be one of the following:

- **Integer Types**: `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `ulong`
- **Floating-point Types**: `float`, `double`

*Note: The documentation below uses a generic syntax `<T>` for brevity, but the actual implementation uses explicit overloads for the types listed above.*

## Methods

### **Sample(MinMax&lt;T&gt;)**

Draws a uniform random value from the [MinMax](../../sdk-cs/shadop-archmage-sdk-minmax-1/) range using Unity's global `UnityEngine.Random`. The returned value lies in `[Min, Max]` (both ends included).

For integer types, this maps to `Random.Range(0, span + 1)` shifted by `Min`. For floating-point types, this uses `Random.value * (Max - Min)` shifted by `Min`.

```csharp
public static T Sample(this MinMax<T> mm)
```

#### Returns

`T`<br>
A random value in the range `[mm.Min, mm.Max]`.
