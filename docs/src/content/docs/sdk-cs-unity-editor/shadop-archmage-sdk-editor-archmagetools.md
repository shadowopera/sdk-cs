---
title: 'ArchmageTools'
description: 'Editor utility methods for Archmage, including DrawEasyDropdown for config ID fields.'
---

Namespace: Shadop.Archmage.Sdk.Editor

Static editor utility class.

```csharp
public static partial class ArchmageTools
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ArchmageTools](.)

## Methods

### **DrawEasyDropdown\<TValue\>(Rect, SerializedProperty, GUIContent, String, TValue[], String[], Vector2)**

Renders a strongly-typed `AdvancedDropdown` in the Unity Inspector for config ID selection with automatic serialization. Supported `TValue` types: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `string`.

```csharp
public static void DrawEasyDropdown<TValue>(
    Rect position, SerializedProperty property, GUIContent label,
    string header,
    TValue[] values, string[] displayNames = null,
    Vector2 minWindowSize = default)
```

#### Parameters

`position` [Rect](https://docs.unity3d.com/ScriptReference/Rect.html)<br>
Rectangle on the screen to use for the property GUI.

`property` [SerializedProperty](https://docs.unity3d.com/ScriptReference/SerializedProperty.html)<br>
SerializedProperty of the field to modify.

`label` [GUIContent](https://docs.unity3d.com/ScriptReference/GUIContent.html)<br>
The label of the property.

`header` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The header text displayed at the top of the dropdown window.

`values` `TValue[]`<br>
The array of configuration ID values.

`displayNames` [String\[\]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Optional: custom display strings. Length must match `values` if provided.

`minWindowSize` [Vector2](https://docs.unity3d.com/ScriptReference/Vector2.html)<br>
Optional: minimum size for the dropdown window. Defaults to `(180, 260)`.
