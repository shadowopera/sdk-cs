---
title: 'Tup2<T0, T1>'
---

Namespace: Shadop.Archmage

0-based tuple with 2 elements. JSON serializes as {"item0": ..., "item1": ...}.

```csharp
public class Tup2<T0, T1> : 
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Tup2<T0, T1>](./shadop.archmage.tup2-2.md)<br>
Implements IEquatable<Tup2<T0, T1>><br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### **Item0**

```csharp
public T0 Item0 { get; set; }
```

#### Property Value

T0<br>

### **Item1**

```csharp
public T1 Item1 { get; set; }
```

#### Property Value

T1<br>

## Constructors

### **Tup2(T0, T1)**

```csharp
public Tup2(T0 item0, T1 item1)
```

#### Parameters

`item0` T0<br>

`item1` T1<br>

## Methods

### **Values()**

```csharp
public Object[] Values()
```

#### Returns

[Object[]](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **Deconstruct(T0&, T1&)**

```csharp
public void Deconstruct(T0& item0, T1& item1)
```

#### Parameters

`item0` T0&<br>

`item1` T1&<br>

### **Equals(Tup2<T0, T1>)**

```csharp
public bool Equals(Tup2<T0, T1> other)
```

#### Parameters

`other` [Tup2<T0, T1>](./shadop.archmage.tup2-2.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
