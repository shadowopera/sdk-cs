---
title: 'Vec3<T>'
---

Namespace: Shadop.Archmage

Represents a 3D vector. Serialized as JSON array [x, y, z].

```csharp
public struct Vec3<T>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Vec3<T>](./shadop.archmage.vec3-1.md)<br>
Implements IEquatable<Vec3<T>><br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md), JsonConverterAttribute

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

### **Z**

```csharp
public T Z { get; set; }
```

#### Property Value

T<br>

## Constructors

### **Vec3(T, T, T)**

```csharp
Vec3(T x, T y, T z)
```

#### Parameters

`x` T<br>

`y` T<br>

`z` T<br>

## Methods

### **Equals(Vec3<T>)**

```csharp
bool Equals(Vec3<T> other)
```

#### Parameters

`other` [Vec3<T>](./shadop.archmage.vec3-1.md)<br>

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
