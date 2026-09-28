---
title: 'UnityJsonSettingsFactory'
---

Namespace: Shadop.Archmage.Sdk

Factory for creating JsonSerializerSettings pre-configured with Unity vector converters.

```csharp
public static class UnityJsonSettingsFactory
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityJsonSettingsFactory](../shadop-archmage-sdk-unityjsonsettingsfactory/)

## Methods

### **Create(JsonSerializerSettings)**

Creates JsonSerializerSettings pre-configured with Unity vector converters.
 Supports: Vector2, Vector3, Vector4, Vector2Int, Vector3Int.
 Each vector type serializes to/from a JSON object: {"x": x, "y": y} etc.

```csharp
public static JsonSerializerSettings Create(JsonSerializerSettings baseSettings)
```

#### Parameters

`baseSettings` JsonSerializerSettings<br>
Optional base settings to clone and extend. If null, new settings are created.

#### Returns

JsonSerializerSettings<br>
JsonSerializerSettings with Unity vector converters registered.
