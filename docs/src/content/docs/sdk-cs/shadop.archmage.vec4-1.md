---
title: 'Vec4<T>'
---

Namespace: Shadop.Archmage

Represents a 4D vector. Serialized as JSON array [x, y, z, w].

```csharp
public struct Vec4<T>
```

#### Type Parameters

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Vec4<T>](./shadop.archmage.vec4-1.md)<br>
Implements IEquatable<Vec4<T>><br>
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

### **W**

```csharp
public T W { get; set; }
```

#### Property Value

T<br>

## Constructors

### **Vec4(T, T, T, T)**

```csharp
Vec4(T x, T y, T z, T w)
```

#### Parameters

`x` T<br>

`y` T<br>

`z` T<br>

`w` T<br>

## Methods

### **Equals(Vec4<T>)**

```csharp
bool Equals(Vec4<T> other)
```

#### Parameters

`other` [Vec4<T>](./shadop.archmage.vec4-1.md)<br>

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
