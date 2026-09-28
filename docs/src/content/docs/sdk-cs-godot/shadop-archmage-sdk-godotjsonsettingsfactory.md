---
title: 'GodotJsonSettingsFactory'
---

Namespace: Shadop.Archmage.Sdk

Factory for creating JsonSerializerSettings pre-configured with Godot vector converters.

```csharp
public static class GodotJsonSettingsFactory
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotJsonSettingsFactory](../shadop-archmage-sdk-godotjsonsettingsfactory/)

## Methods

### **Create(JsonSerializerSettings)**

Creates JsonSerializerSettings pre-configured with Godot vector converters.
 Supports: Vector2, Vector3, Vector4, Vector2I, Vector3I, Vector4I.
 Each vector type serializes to/from a JSON object: {"x": x, "y": y} etc.

```csharp
public static JsonSerializerSettings Create(JsonSerializerSettings baseSettings)
```

#### Parameters

`baseSettings` JsonSerializerSettings<br>
Optional base settings to clone and extend. If null, new settings are created.

#### Returns

JsonSerializerSettings<br>
JsonSerializerSettings with Godot vector converters registered.
