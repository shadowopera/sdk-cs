---
title: 'GodotVecExtensions'
description: 'Extension methods for converting Vec2, Vec3, and Vec4 to Godot Vector types.'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for [Vec2](../../sdk-cs/shadop-archmage-sdk-vec2-1/), [Vec3](../../sdk-cs/shadop-archmage-sdk-vec3-1/), and [Vec4](../../sdk-cs/shadop-archmage-sdk-vec4-1/).

```csharp
public static class GodotVecExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotVecExtensions](.)

## Supported Types

The underlying generic type `T` can be one of the following:

- **Integer Types**: `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`
- **Floating-point Types**: `float`, `double`

*Note: The documentation below uses a generic syntax `<T>` for brevity, but the actual implementation uses explicit overloads for the types listed above.*

## Methods

### **ToVector2(Vec2&lt;T&gt;)**

Converts a floating-point [Vec2](../../sdk-cs/shadop-archmage-sdk-vec2-1/) to a `Godot.Vector2`. Supported for `float` and `double`.

```csharp
public static Vector2 ToVector2(this Vec2<T> vec)
```

#### Returns

[Vector2](https://docs.godotengine.org/en/stable/classes/class_vector2.html)<br>

---

### **ToVector2I(Vec2&lt;T&gt;)**

Converts an integer [Vec2](../../sdk-cs/shadop-archmage-sdk-vec2-1/) to a `Godot.Vector2I`. Supported for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, and `ulong`.
`uint`, `long`, and `ulong` components are cast to `int`.

```csharp
public static Vector2I ToVector2I(this Vec2<T> vec)
```

#### Returns

[Vector2I](https://docs.godotengine.org/en/stable/classes/class_vector2i.html)<br>

---

### **ToVector3(Vec3&lt;T&gt;)**

Converts a floating-point [Vec3](../../sdk-cs/shadop-archmage-sdk-vec3-1/) to a `Godot.Vector3`. Supported for `float` and `double`.

```csharp
public static Vector3 ToVector3(this Vec3<T> vec)
```

#### Returns

[Vector3](https://docs.godotengine.org/en/stable/classes/class_vector3.html)<br>

---

### **ToVector3I(Vec3&lt;T&gt;)**

Converts an integer [Vec3](../../sdk-cs/shadop-archmage-sdk-vec3-1/) to a `Godot.Vector3I`. Supported for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, and `ulong`.
`uint`, `long`, and `ulong` components are cast to `int`.

```csharp
public static Vector3I ToVector3I(this Vec3<T> vec)
```

#### Returns

[Vector3I](https://docs.godotengine.org/en/stable/classes/class_vector3i.html)<br>

---

### **ToVector4(Vec4&lt;T&gt;)**

Converts a floating-point [Vec4](../../sdk-cs/shadop-archmage-sdk-vec4-1/) to a `Godot.Vector4`. Supported for `float` and `double`.

```csharp
public static Vector4 ToVector4(this Vec4<T> vec)
```

#### Returns

[Vector4](https://docs.godotengine.org/en/stable/classes/class_vector4.html)<br>

---

### **ToVector4I(Vec4&lt;T&gt;)**

Converts an integer [Vec4](../../sdk-cs/shadop-archmage-sdk-vec4-1/) to a `Godot.Vector4I`. Supported for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, and `ulong`.
`uint`, `long`, and `ulong` components are cast to `int`.

```csharp
public static Vector4I ToVector4I(this Vec4<T> vec)
```

#### Returns

[Vector4I](https://docs.godotengine.org/en/stable/classes/class_vector4i.html)<br>
