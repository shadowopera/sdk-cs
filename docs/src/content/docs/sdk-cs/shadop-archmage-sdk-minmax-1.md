---
title: 'MinMax<T>'
---

Namespace: Shadop.Archmage.Sdk

A numeric range bounded by [MinMax<T>.Min](../shadop-archmage-sdk-minmax-1/#min) and [MinMax<T>.Max](../shadop-archmage-sdk-minmax-1/#max). The `Sample`
 extension methods (in [MinMaxExtensions](../shadop-archmage-sdk-minmaxextensions/)) can draw a random value from it.

```csharp
public struct MinMax<T>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [MinMax<T>](../shadop-archmage-sdk-minmax-1/)<br>
Implements IEquatable<MinMax<T>>

## Properties

### **Min**

```csharp
public T Min { get; set; }
```

#### Property Value

T<br>

### **Max**

```csharp
public T Max { get; set; }
```

#### Property Value

T<br>

## Constructors

### **MinMax(T, T)**

```csharp
MinMax(T min, T max)
```

#### Parameters

`min` T<br>

`max` T<br>

## Methods

### **Equals(MinMax<T>)**

```csharp
bool Equals(MinMax<T> other)
```

#### Parameters

`other` [MinMax<T>](../shadop-archmage-sdk-minmax-1/)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
