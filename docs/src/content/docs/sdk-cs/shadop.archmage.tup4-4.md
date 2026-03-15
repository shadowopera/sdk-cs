---
title: 'Tup4<T0, T1, T2, T3>'
---

Namespace: Shadop.Archmage

0-based tuple with 4 elements. JSON serializes as {"item0": ..., "item1": ..., "item2": ..., "item3": ...}.

```csharp
public class Tup4<T0, T1, T2, T3> : 
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Tup4<T0, T1, T2, T3>](./shadop.archmage.tup4-4.md)<br>
Implements IEquatable<Tup4<T0, T1, T2, T3>><br>
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

### **Item2**

```csharp
public T2 Item2 { get; set; }
```

#### Property Value

T2<br>

### **Item3**

```csharp
public T3 Item3 { get; set; }
```

#### Property Value

T3<br>

## Constructors

### **Tup4(T0, T1, T2, T3)**

```csharp
public Tup4(T0 item0, T1 item1, T2 item2, T3 item3)
```

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

`item3` T3<br>

## Methods

### **Values()**

```csharp
public Object[] Values()
```

#### Returns

[Object[]](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **Deconstruct(T0&, T1&, T2&, T3&)**

```csharp
public void Deconstruct(T0& item0, T1& item1, T2& item2, T3& item3)
```

#### Parameters

`item0` T0&<br>

`item1` T1&<br>

`item2` T2&<br>

`item3` T3&<br>

### **Equals(Tup4<T0, T1, T2, T3>)**

```csharp
public bool Equals(Tup4<T0, T1, T2, T3> other)
```

#### Parameters

`other` [Tup4<T0, T1, T2, T3>](./shadop.archmage.tup4-4.md)<br>

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
