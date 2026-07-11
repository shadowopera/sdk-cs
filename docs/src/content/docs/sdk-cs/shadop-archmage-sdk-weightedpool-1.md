---
title: 'WeightedPool<T>'
---

Namespace: Shadop.Archmage.Sdk

Holds items alongside their selection weights in two parallel arrays of equal length.
 The `Sample` and `SampleIndex` extension methods (in
 [WeightedPoolExtensions](../shadop-archmage-sdk-weightedpoolextensions/)) draw an item at random with probability
 proportional to its weight. The `SampleWith` and `SampleIndexWith`
 methods map a caller-supplied value to an item deterministically according to the weights.

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

### **SampleWith(Single)**

Maps the `value` to an item deterministically according to the weights.
 `value < 0` returns the first item with non-zero weight; `value >= 1`
 returns the last. Throws if the pool is empty or the total weight is zero.

```csharp
public T SampleWith(float value)
```

#### Parameters

`value` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

T<br>

### **SampleIndexWith(Single)**

Maps the `value` to an item index deterministically according to the weights.
 `value < 0` returns the first item index with non-zero weight; `value >= 1`
 returns the last. Throws if the pool is empty or the total weight is zero.

```csharp
public int SampleIndexWith(float value)
```

#### Parameters

`value` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
