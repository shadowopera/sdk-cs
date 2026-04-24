---
title: 'VecExtensions'
description: 'Extension methods for converting Vec2, Vec3, and Vec4 to Unity Vector types.'
---

Namespace: Shadop.Archmage.Sdk

Extension methods for [Vec2](../../sdk-cs/shadop-archmage-sdk-vec2-1/), [Vec3](../../sdk-cs/shadop-archmage-sdk-vec3-1/), and [Vec4](../../sdk-cs/shadop-archmage-sdk-vec4-1/).

```csharp
public static class VecExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [VecExtensions](.)

## Supported Types

To maximize performance and avoid garbage collection (GC) allocations in Unity, these extensions provide explicit, hardcoded overloads for standard numeric types.

The underlying generic type `T` can be one of the following:

- **Integer Types**: `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`
- **Floating-point Types**: `float`, `double`

*Note: The documentation below uses a generic syntax `<T>` for brevity, but the actual implementation uses explicit overloads for the types listed above.*

## Methods

### **ToVector2(Vec2&lt;T&gt;)**

Converts a floating-point [Vec2](../../sdk-cs/shadop-archmage-sdk-vec2-1/) to a `UnityEngine.Vector2`. Supported for `float` and `double`.

```csharp
public static Vector2 ToVector2(this Vec2<T> vec)
```

#### Returns

[Vector2](https://docs.unity3d.com/ScriptReference/Vector2.html)<br>

---

### **ToVector2Int(Vec2&lt;T&gt;)**

Converts an integer [Vec2](../../sdk-cs/shadop-archmage-sdk-vec2-1/) to a `UnityEngine.Vector2Int`. Supported for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, and `ulong`.

```csharp
public static Vector2Int ToVector2Int(this Vec2<T> vec)
```

#### Returns

[Vector2Int](https://docs.unity3d.com/ScriptReference/Vector2Int.html)<br>

---

### **ToVector3(Vec3&lt;T&gt;)**

Converts a floating-point [Vec3](../../sdk-cs/shadop-archmage-sdk-vec3-1/) to a `UnityEngine.Vector3`. Supported for `float` and `double`.

```csharp
public static Vector3 ToVector3(this Vec3<T> vec)
```

#### Returns

[Vector3](https://docs.unity3d.com/ScriptReference/Vector3.html)<br>

---

### **ToVector3Int(Vec3&lt;T&gt;)**

Converts an integer [Vec3](../../sdk-cs/shadop-archmage-sdk-vec3-1/) to a `UnityEngine.Vector3Int`. Supported for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, and `ulong`.

```csharp
public static Vector3Int ToVector3Int(this Vec3<T> vec)
```

#### Returns

[Vector3Int](https://docs.unity3d.com/ScriptReference/Vector3Int.html)<br>

---

### **ToVector4(Vec4&lt;T&gt;)**

Converts a [Vec4](../../sdk-cs/shadop-archmage-sdk-vec4-1/) to a `UnityEngine.Vector4`. Because Unity does not have a standard `Vector4Int` type, this method is supported for **all** numeric types (both integer and floating-point).

```csharp
public static Vector4 ToVector4(this Vec4<T> vec)
```

#### Returns

[Vector4](https://docs.unity3d.com/ScriptReference/Vector4.html)<br>
