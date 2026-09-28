---
title: 'GodotMinMaxExtensions'
description: 'Extension methods for sampling a uniform random value from a MinMax<T> range using Godot''s global random number generator.'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for [MinMax](../../sdk-cs/shadop-archmage-sdk-minmax-1/).

```csharp
public static class GodotMinMaxExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotMinMaxExtensions](.)

## Supported Types

The underlying generic type `T` can be one of the following:

- **Integer Types**: `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `ulong`
- **Floating-point Types**: `float`, `double`
- **Duration**: [Duration](../../sdk-cs/shadop-archmage-sdk-duration/)

*Note: The documentation below uses a generic syntax `<T>` for brevity, but the actual implementation uses explicit overloads for the types listed above.*

## Methods

### **Sample(MinMax&lt;T&gt;)**

Draws a uniform random value from the [MinMax](../../sdk-cs/shadop-archmage-sdk-minmax-1/) range using Godot's global random number generator, which `GD.Seed` affects. The returned value lies in `[Min, Max]` (both ends included).

For integer types, this maps to `GD.RandRange(0, Max - Min)` shifted by `Min`. For floating-point types, this uses `GD.Randf() * (Max - Min)` shifted by `Min`. For `Duration`, the draw has millisecond precision.

```csharp
public static T Sample(this MinMax<T> mm)
```

#### Returns

`T`<br>
A random value in the range `[mm.Min, mm.Max]`.
