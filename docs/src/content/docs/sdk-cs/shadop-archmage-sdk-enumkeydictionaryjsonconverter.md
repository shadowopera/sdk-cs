---
title: 'EnumKeyDictionaryJsonConverter'
---

Namespace: Shadop.Archmage.Sdk

Newtonsoft.Json converter that writes enum-keyed dictionaries with numeric keys,
 the same keys archmage exports. Reading uses the default handling, which accepts numeric keys.

```csharp
public class EnumKeyDictionaryJsonConverter : Newtonsoft.Json.JsonConverter
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → [EnumKeyDictionaryJsonConverter](../shadop-archmage-sdk-enumkeydictionaryjsonconverter/)<br>

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

### **EnumKeyDictionaryJsonConverter()**

```csharp
public EnumKeyDictionaryJsonConverter()
```

## Methods

### **CanConvert(Type)**

```csharp
public bool CanConvert(Type objectType)
```

#### Parameters

`objectType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ReadJson(JsonReader, Type, Object, JsonSerializer)**

```csharp
public object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
```

#### Parameters

`reader` JsonReader<br>

`objectType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`existingValue` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`serializer` JsonSerializer<br>

#### Returns

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **WriteJson(JsonWriter, Object, JsonSerializer)**

Serializes the dictionary, writing each key as the enum's underlying integer value.

```csharp
public void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
```

#### Parameters

`writer` JsonWriter<br>

`value` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`serializer` JsonSerializer<br>
