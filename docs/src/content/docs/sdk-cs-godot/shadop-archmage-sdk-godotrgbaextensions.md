---
title: 'GodotRgbaExtensions'
description: 'Extension methods for converting Rgba to Godot.Color.'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for [Rgba](../../sdk-cs/shadop-archmage-sdk-rgba/).

```csharp
public static class GodotRgbaExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotRgbaExtensions](.)

## Methods

### **ToColor(Rgba)**

Converts an [Rgba](../../sdk-cs/shadop-archmage-sdk-rgba/) value to a `Godot.Color`.
Each channel is mapped from [0, 255] to [0, 1].

```csharp
public static Color ToColor(this Rgba rgba)
```

#### Parameters

`rgba` [Rgba](../../sdk-cs/shadop-archmage-sdk-rgba/)<br>

#### Returns

[Color](https://docs.godotengine.org/en/stable/classes/class_color.html)<br>
