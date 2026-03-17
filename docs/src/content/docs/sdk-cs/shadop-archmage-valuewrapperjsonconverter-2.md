---
title: 'ValueWrapperJsonConverter<T, V>'
---

Namespace: Shadop.Archmage

JSON converter for value wrapper structs that hold a single  value.

```csharp
public abstract class ValueWrapperJsonConverter<T, V> : 
```

#### Type Parameters

`T`<br>
The value wrapper struct type.

`V`<br>
The underlying value type.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter<T> → [ValueWrapperJsonConverter<T, V>](./shadop.archmage.valuewrapperjsonconverter-2.md)

## Properties

### **CanRead**

```csharp
public bool CanRead { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **CanWrite**

```csharp
public bool CanWrite { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **ValueWrapperJsonConverter()**

```csharp
protected ValueWrapperJsonConverter()
```

## Methods

### **Create(V)**

```csharp
protected abstract T Create(V value)
```

#### Parameters

`value` V<br>

#### Returns

T<br>

### **GetValue(T)**

```csharp
protected abstract V GetValue(T obj)
```

#### Parameters

`obj` T<br>

#### Returns

V<br>

### **ReadJson(JsonReader, Type, T, Boolean, JsonSerializer)**

```csharp
public T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
```

#### Parameters

`reader` JsonReader<br>

`objectType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`existingValue` T<br>

`hasExistingValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`serializer` JsonSerializer<br>

#### Returns

T<br>

### **WriteJson(JsonWriter, T, JsonSerializer)**

```csharp
public void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
```

#### Parameters

`writer` JsonWriter<br>

`value` T<br>

`serializer` JsonSerializer<br>
