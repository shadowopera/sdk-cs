---
title: 'AtlasItem'
---

Namespace: Shadop.Archmage

Configuration item within an Atlas.

```csharp
public class AtlasItem
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AtlasItem](./shadop.archmage.atlasitem.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### **Cfg**

The deserialized configuration object. Initialize to target type instance before loading.

```csharp
public object Cfg { get; set; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **Mapping**

The mapping strategy: "unique", "single", or "multiple".

```csharp
public string Mapping { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Key**

The item's key in atlas.json.

```csharp
public string Key { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Ready**

Whether this item has been successfully loaded.

```csharp
public bool Ready { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **AtlasItem()**

```csharp
public AtlasItem()
```
