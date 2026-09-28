---
title: 'GodotRgbaExtensions'
---

Namespace: Shadop.Archmage.Sdk

```csharp
public static class GodotRgbaExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotRgbaExtensions](../shadop-archmage-sdk-godotrgbaextensions/)<br>

## Methods

### **ToColor(Rgba)**

Converts an [Rgba](../../sdk-cs/shadop-archmage-sdk-rgba/) value to a `Godot.Color`.
 Each channel is mapped from [0, 255] to [0, 1].

```csharp
public static Color ToColor(Rgba rgba)
```

#### Parameters

`rgba` [Rgba](../../sdk-cs/shadop-archmage-sdk-rgba/)<br>

#### Returns

Color<br>
