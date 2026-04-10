---
title: 'CfgIdDrawer<TValue>'
description: 'Abstract base PropertyDrawer that renders an EasyDropdown for config ID fields in the Unity Inspector.'
---

Namespace: Shadop.Archmage.Sdk.Editor

Abstract base for config ID property drawers. Renders an `ArchmageTools.DrawEasyDropdown<TValue>` dropdown in the Inspector. Subclasses provide the ID values and display names, typically populated from a config table at editor load time.

```csharp
public abstract class CfgIdDrawer<TValue> : PropertyDrawer
    where TValue : IComparable<TValue>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PropertyDrawer](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) → [CfgIdDrawer\<TValue\>](.)

## Methods

### **GetHeader()**

Returns the header text displayed at the top of the dropdown window.

```csharp
protected abstract string GetHeader()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetIdValues()**

Returns the array of config ID values to populate the dropdown.

```csharp
protected abstract TValue[] GetIdValues()
```

#### Returns

`TValue[]`<br>

### **GetDisplayNames()**

Returns the array of display strings corresponding to each ID value.

```csharp
protected abstract string[] GetDisplayNames()
```

#### Returns

[String\[\]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **BuildDisplayNames(TValue[], Func\<TValue, String\>, Int32)**

Builds a display-name array from `values`. Entries before `start` are left as `null` so the caller can fill them manually (e.g., a "Default" label at index 0).

```csharp
protected static string[] BuildDisplayNames(TValue[] values, Func<TValue, string> format, int start)
```

#### Parameters

`values` `TValue[]`<br>
The ID values array.

`format` [Func\<TValue, String\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Formatter that converts an ID value to its display string.

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Index to start formatting from.

#### Returns

[String\[\]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **OnGUI(Rect, SerializedProperty, GUIContent)**

```csharp
public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
```

#### Parameters

`position` [Rect](https://docs.unity3d.com/ScriptReference/Rect.html)<br>

`property` [SerializedProperty](https://docs.unity3d.com/ScriptReference/SerializedProperty.html)<br>

`label` [GUIContent](https://docs.unity3d.com/ScriptReference/GUIContent.html)<br>
