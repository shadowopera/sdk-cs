---
title: 'Tup3<T0, T1, T2>'
---

Namespace: Shadop.Archmage.Sdk

0-based tuple with 3 elements. JSON serializes as {"item0": ..., "item1": ..., "item2": ...}.

```csharp
public class Tup3<T0, T1, T2> : 
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Tup3<T0, T1, T2>](../shadop-archmage-sdk-tup3-3/)<br>
Implements IEquatable<Tup3<T0, T1, T2>><br>

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

## Constructors

### **Tup3(T0, T1, T2)**

```csharp
public Tup3(T0 item0, T1 item1, T2 item2)
```

#### Parameters

`item0` T0<br>

`item1` T1<br>

`item2` T2<br>

## Methods

### **Values()**

```csharp
public Object[] Values()
```

#### Returns

[Object[]](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **Deconstruct(T0&, T1&, T2&)**

```csharp
public void Deconstruct(T0& item0, T1& item1, T2& item2)
```

#### Parameters

`item0` T0&<br>

`item1` T1&<br>

`item2` T2&<br>

### **Equals(Tup3<T0, T1, T2>)**

```csharp
public bool Equals(Tup3<T0, T1, T2> other)
```

#### Parameters

`other` [Tup3<T0, T1, T2>](../shadop-archmage-sdk-tup3-3/)<br>

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
