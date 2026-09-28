---
title: 'GodotWeightedPoolExtensions'
---

Namespace: Shadop.Archmage.Sdk

Provides parameterless `Sample` and `SampleIndex` extension methods that draw an
 item from a [WeightedPool<T>](../../sdk-cs/shadop-archmage-sdk-weightedpool-1/) at random with probability proportional to its
 weight, using Godot's global random number generator, which `GD.Seed` affects.

```csharp
public static class GodotWeightedPoolExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotWeightedPoolExtensions](../shadop-archmage-sdk-godotweightedpoolextensions/)<br>

## Methods

### **Sample<T>(WeightedPool<T>)**

Returns a randomly selected item, weighted by the pool's weights.
 Throws if the pool is empty or the total weight is zero.

```csharp
public static T Sample<T>(WeightedPool<T> wp)
```

#### Parameters

`wp` WeightedPool<T><br>

#### Returns

T<br>

### **SampleIndex<T>(WeightedPool<T>)**

Returns the index of a randomly selected item, weighted by the pool's weights.
 Throws if the pool is empty or the total weight is zero.

```csharp
public static int SampleIndex<T>(WeightedPool<T> wp)
```

#### Parameters

`wp` WeightedPool<T><br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
