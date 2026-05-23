---
title: 'Tup2<T0, T1>'
---

Namespace: Shadop.Archmage.Sdk

0-based tuple with 2 elements. JSON serializes as {"item0": ..., "item1": ...}.

```csharp
public class Tup2<T0, T1>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Tup2<T0, T1>](../shadop-archmage-sdk-tup2-2/)<br>

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

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
