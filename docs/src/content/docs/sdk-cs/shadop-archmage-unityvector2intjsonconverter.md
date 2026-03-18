---
title: 'UnityVector2IntJsonConverter'
---

Namespace: Shadop.Archmage

JSON converter for UnityEngine.Vector2Int.
Serializes to/from `[x, y]` array format.
Null values deserialize to Vector2Int.zero.

```csharp
public class UnityVector2IntJsonConverter : JsonConverter
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [JsonConverter](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonConverter.htm) → [UnityVector2IntJsonConverter](./shadop-archmage-unityvector2intjsonconverter.md)<br>
Attributes [NullableContextAttribute](../sdk-cs/system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](../sdk-cs/system.runtime.compilerservices.nullableattribute.md)

## Constructors

### **UnityVector2IntJsonConverter()**

```csharp
public UnityVector2IntJsonConverter()
```

## Methods

### **CanConvert(Type)**

```csharp
public override bool CanConvert(Type objectType)
```

#### Parameters

`objectType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ReadJson(JsonReader, Type, Object, JsonSerializer)**

```csharp
public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
```

#### Parameters

`reader` [JsonReader](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonReader.htm)<br>

`objectType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`existingValue` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`serializer` [JsonSerializer](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializer.htm)<br>

#### Returns

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **WriteJson(JsonWriter, Object, JsonSerializer)**

```csharp
public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
```

#### Parameters

`writer` [JsonWriter](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonWriter.htm)<br>

`value` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`serializer` [JsonSerializer](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializer.htm)<br>
