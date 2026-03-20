---
title: 'UnityJsonSettingsFactory'
description: 'Factory for creating JsonSerializerSettings with Unity vector converters pre-registered.'
---

Namespace: Shadop.Archmage

Factory for creating `JsonSerializerSettings` pre-configured with Unity vector converters.
Supports: `Vector2`, `Vector3`, `Vector4`, `Vector2Int`, `Vector3Int`.

```csharp
public static class UnityJsonSettingsFactory
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityJsonSettingsFactory](.)

## Methods

### **Create(JsonSerializerSettings)**

Creates JsonSerializerSettings pre-configured with Unity vector converters.
Each vector type serializes to/from a JSON array: `[x, y]`, `[x, y, z]`, or `[x, y, z, w]`.

```csharp
public static JsonSerializerSettings Create(JsonSerializerSettings? baseSettings = null)
```

#### Parameters

`baseSettings` [JsonSerializerSettings](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializerSettings.htm)<br>
Optional base settings to clone and extend. If null, new settings are created.

#### Returns

[JsonSerializerSettings](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializerSettings.htm)<br>
JsonSerializerSettings with Unity vector converters registered.
