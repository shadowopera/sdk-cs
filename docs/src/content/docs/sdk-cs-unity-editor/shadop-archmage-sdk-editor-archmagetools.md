---
title: 'ArchmageTools'
---

Namespace: Shadop.Archmage.Sdk.Editor

```csharp
public static class ArchmageTools
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ArchmageTools](../shadop-archmage-sdk-editor-archmagetools/)<br>

## Methods

### **DrawEasyDropdown<TValue>(Rect, SerializedProperty, GUIContent, String, TValue[], String[], Vector2)**

Draws a strongly-typed AdvancedDropdown for configurations with automatic serialization.

```csharp
public static void DrawEasyDropdown<TValue>(Rect position, SerializedProperty property, GUIContent label, string header, TValue[] values, String[] displayNames, Vector2 minWindowSize)
```

#### Type Parameters

`TValue`<br>
The underlying data type of the ID (supports: sbyte, byte, short, ushort, int, uint, long, ulong, string).

#### Parameters

`position` Rect<br>
Rectangle on the screen to use for the property GUI.

`property` SerializedProperty<br>
SerializedProperty of the field to modify.

`label` GUIContent<br>
The label of the property.

`header` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The header text displayed at the top of the dropdown window.

`values` TValue[]<br>
The array of configuration ID values.

`displayNames` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Optional: custom display strings. Length must match values if provided.

`minWindowSize` Vector2<br>
Optional: minimum size for dropdown window. Defaults to `(180, 260)`.
