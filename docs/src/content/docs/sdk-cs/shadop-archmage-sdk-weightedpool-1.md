---
title: 'WeightedPool<T>'
---

Namespace: Shadop.Archmage.Sdk

Holds items alongside their selection weights in two parallel arrays of equal length.
 The `Sample` and `SampleIndex` extension methods (in
 [WeightedPoolExtensions](../shadop-archmage-sdk-weightedpoolextensions/)) draw an item at random with probability
 proportional to its weight. The `SampleWithNoise` and `SampleIndexWithNoise`
 methods map a caller-supplied noise value across the cumulative weights instead.

```csharp
public class WeightedPool<T>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [WeightedPool<T>](../shadop-archmage-sdk-weightedpool-1/)<br>

## Properties

### **Items**

The candidate values, one per weight.

```csharp
public T[] Items { get; set; }
```

#### Property Value

T[]<br>

### **Weights**

The non-negative selection weights, parallel to [WeightedPool<T>.Items](../shadop-archmage-sdk-weightedpool-1/#items).

```csharp
public Int32[] Weights { get; set; }
```

#### Property Value

[Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Count**

The number of items in the pool.

```csharp
public int Count { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **WeightedPool()**

```csharp
public WeightedPool()
```

### **WeightedPool(T[], Int32[])**

```csharp
public WeightedPool(T[] items, Int32[] weights)
```

#### Parameters

`items` T[]<br>

`weights` [Int32[]](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **SampleWithNoise(Single)**

Maps the `noise` value (0 to 1) to an item according to the weights.
 `noise < 0` returns the first item with non-zero weight; `noise >= 1`
 returns the last. Throws if the pool is empty or the total weight is zero.

```csharp
public T SampleWithNoise(float noise)
```

#### Parameters

`noise` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

T<br>

### **SampleIndexWithNoise(Single)**

Maps the `noise` value (0 to 1) to an item index according to the weights.
 `noise < 0` returns the first index with non-zero weight; `noise >= 1`
 returns the last. Throws if the pool is empty or the total weight is zero.

```csharp
public int SampleIndexWithNoise(float noise)
```

#### Parameters

`noise` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
