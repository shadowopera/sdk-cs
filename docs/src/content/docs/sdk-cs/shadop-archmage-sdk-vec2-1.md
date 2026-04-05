---
title: 'Vec2<T>'
---

Namespace: Shadop.Archmage.Sdk

Represents a 2D vector. Serialized as JSON array [x, y].

```csharp
public struct Vec2<T>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Vec2<T>](../shadop-archmage-sdk-vec2-1/)<br>
Implements IEquatable<Vec2<T>><br>

## Properties

### **X**

```csharp
public T X { get; set; }
```

#### Property Value

T<br>

### **Y**

```csharp
public T Y { get; set; }
```

#### Property Value

T<br>

## Constructors

### **Vec2(T, T)**

```csharp
Vec2(T x, T y)
```

#### Parameters

`x` T<br>

`y` T<br>

## Methods

### **Equals(Vec2<T>)**

```csharp
bool Equals(Vec2<T> other)
```

#### Parameters

`other` [Vec2<T>](../shadop-archmage-sdk-vec2-1/)<br>

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
