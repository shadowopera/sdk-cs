---
title: 'UnityWeightedPoolExtensions'
description: 'Extension methods for drawing a random item from a WeightedPool<T> using Unity''s global Random.'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for [WeightedPool&lt;T&gt;](../../sdk-cs/shadop-archmage-sdk-weightedpool-1/).

```csharp
public static class UnityWeightedPoolExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityWeightedPoolExtensions](.)

## Methods

### **Sample&lt;T&gt;(WeightedPool&lt;T&gt;)**

Returns a randomly selected item, with each item's probability proportional to its weight. Uses Unity's global `UnityEngine.Random`.

Throws if the pool is empty or the total weight is zero.

```csharp
public static T Sample<T>(this WeightedPool<T> wp)
```

#### Returns

`T`<br>
The randomly selected item.

---

### **SampleIndex&lt;T&gt;(WeightedPool&lt;T&gt;)**

Returns the index of a randomly selected item, with each item's probability proportional to its weight. Uses Unity's global `UnityEngine.Random`.

Throws if the pool is empty or the total weight is zero.

```csharp
public static int SampleIndex<T>(this WeightedPool<T> wp)
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The zero-based index of the randomly selected item.
