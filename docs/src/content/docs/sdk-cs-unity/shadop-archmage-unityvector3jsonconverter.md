---
title: 'UnityVector3JsonConverter'
description: 'Newtonsoft.Json converter for UnityEngine.Vector3. Serializes to/from [x, y, z] array format.'
---

Namespace: Shadop.Archmage

JSON converter for `UnityEngine.Vector3`.
Serializes to/from `[x, y, z]` array format.
Null values deserialize to `Vector3.zero`.

```csharp
public class UnityVector3JsonConverter : JsonConverter
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [JsonConverter](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonConverter.htm) → [UnityVector3JsonConverter](.)

## Constructors

### **UnityVector3JsonConverter()**

```csharp
public UnityVector3JsonConverter()
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
