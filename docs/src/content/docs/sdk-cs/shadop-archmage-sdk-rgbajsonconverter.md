---
title: 'RgbaJsonConverter'
---

Namespace: Shadop.Archmage.Sdk

Newtonsoft.Json converter for [Rgba](../shadop-archmage-sdk-rgba/).

```csharp
public class RgbaJsonConverter : Newtonsoft.Json.JsonConverter`1[[Shadop.Archmage.Sdk.Rgba, Archmage, Version=0.8.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → JsonConverter → JsonConverter<Rgba> → [RgbaJsonConverter](../shadop-archmage-sdk-rgbajsonconverter/)<br>

**Remarks:**

Serializes as `"#RRGGBBAA"`, or an empty string for the zero value.
 Deserializes from `"#RRGGBB"`, `"#RRGGBBAA"`, empty string, or `null`.

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

### **RgbaJsonConverter()**

```csharp
public RgbaJsonConverter()
```

## Methods

### **ReadJson(JsonReader, Type, Rgba, Boolean, JsonSerializer)**

```csharp
public Rgba ReadJson(JsonReader reader, Type objectType, Rgba existingValue, bool hasExistingValue, JsonSerializer serializer)
```

#### Parameters

`reader` JsonReader<br>

`objectType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

`existingValue` [Rgba](../shadop-archmage-sdk-rgba/)<br>

`hasExistingValue` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`serializer` JsonSerializer<br>

#### Returns

[Rgba](../shadop-archmage-sdk-rgba/)<br>

### **WriteJson(JsonWriter, Rgba, JsonSerializer)**

```csharp
public void WriteJson(JsonWriter writer, Rgba value, JsonSerializer serializer)
```

#### Parameters

`writer` JsonWriter<br>

`value` [Rgba](../shadop-archmage-sdk-rgba/)<br>

`serializer` JsonSerializer<br>
