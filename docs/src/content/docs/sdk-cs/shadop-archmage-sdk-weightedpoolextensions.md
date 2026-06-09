---
title: 'WeightedPoolExtensions'
---

Namespace: Shadop.Archmage.Sdk

Provides `Sample` and `SampleIndex` extension methods that draw an item from a
 [WeightedPool<T>](../shadop-archmage-sdk-weightedpool-1/) at random with probability proportional to its weight,
 using a [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random) source.

```csharp
public static class WeightedPoolExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [WeightedPoolExtensions](../shadop-archmage-sdk-weightedpoolextensions/)<br>

## Methods

### **Sample<T>(WeightedPool<T>, Random)**

Returns a randomly selected item, weighted by the pool's weights.
 Throws if the pool is empty or the total weight is zero.

```csharp
public static T Sample<T>(WeightedPool<T> wp, Random rng)
```

#### Parameters

`wp` WeightedPool<T><br>

`rng` [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

#### Returns

T<br>

### **SampleIndex<T>(WeightedPool<T>, Random)**

Returns the index of a randomly selected item, weighted by the pool's weights.
 Throws if the pool is empty or the total weight is zero.

```csharp
public static int SampleIndex<T>(WeightedPool<T> wp, Random rng)
```

#### Parameters

`wp` WeightedPool<T><br>

`rng` [Random](https://docs.microsoft.com/en-us/dotnet/api/system.random)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
