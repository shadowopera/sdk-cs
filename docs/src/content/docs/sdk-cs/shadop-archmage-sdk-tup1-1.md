---
title: 'Tup1<T0>'
---

Namespace: Shadop.Archmage.Sdk

0-based tuple with 1 element. JSON serializes as {"item0": ...}.

```csharp
public class Tup1<T0> : 
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Tup1<T0>](../shadop-archmage-sdk-tup1-1/)<br>
Implements IEquatable<Tup1<T0>><br>

## Properties

### **Item0**

```csharp
public T0 Item0 { get; set; }
```

#### Property Value

T0<br>

## Constructors

### **Tup1(T0)**

```csharp
public Tup1(T0 item0)
```

#### Parameters

`item0` T0<br>

## Methods

### **Values()**

```csharp
public Object[] Values()
```

#### Returns

[Object[]](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **Deconstruct(T0&)**

```csharp
public void Deconstruct(T0& item0)
```

#### Parameters

`item0` T0&<br>

### **Equals(Tup1<T0>)**

```csharp
public bool Equals(Tup1<T0> other)
```

#### Parameters

`other` [Tup1<T0>](../shadop-archmage-sdk-tup1-1/)<br>

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
