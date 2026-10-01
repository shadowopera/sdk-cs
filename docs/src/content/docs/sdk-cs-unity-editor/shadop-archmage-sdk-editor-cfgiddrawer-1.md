---
title: 'CfgIdDrawer<TValue>'
---

Namespace: Shadop.Archmage.Sdk.Editor

Abstract base for config ID property drawers. Renders an [ArchmageTools.DrawEasyDropdown<TValue>(Rect, SerializedProperty, GUIContent, String, TValue[], String[], Vector2)](../shadop-archmage-sdk-editor-archmagetools/#draweasydropdowntvaluerect-serializedproperty-guicontent-string-tvalue-string-vector2)
 dropdown in the Inspector. Subclasses provide the ID values and display names, typically
 populated from a config table at editor load time.

```csharp
public abstract class CfgIdDrawer<TValue> : UnityEditor.PropertyDrawer
```

#### Type Parameters

`TValue`<br>
The underlying numeric or string type of the config ID.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → GUIDrawer → PropertyDrawer → [CfgIdDrawer<TValue>](../shadop-archmage-sdk-editor-cfgiddrawer-1/)<br>

## Properties

### **attribute**

```csharp
public PropertyAttribute attribute { get; }
```

#### Property Value

PropertyAttribute<br>

### **fieldInfo**

```csharp
public FieldInfo fieldInfo { get; }
```

#### Property Value

[FieldInfo](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldinfo)<br>

### **preferredLabel**

```csharp
public string preferredLabel { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **CfgIdDrawer()**

```csharp
protected CfgIdDrawer()
```

## Methods

### **GetHeader()**

Returns the header text displayed at the top of the dropdown window.

```csharp
protected abstract string GetHeader()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetIdValues()**

Returns the config ID values shown in the dropdown.

```csharp
protected abstract TValue[] GetIdValues()
```

#### Returns

TValue[]<br>

### **GetDisplayNames()**

Returns the display strings, one for each ID value.

```csharp
protected abstract String[] GetDisplayNames()
```

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **BuildDisplayNames(TValue[], Func<TValue, String>, Int32)**

Builds a display-name array from `values`.

```csharp
protected static String[] BuildDisplayNames(TValue[] values, Func<TValue, string> format, int start)
```

#### Parameters

`values` TValue[]<br>
The ID values array.

`format` Func<TValue, String><br>
Formatter that converts an ID value to its display string.

`start` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Index to start formatting from. Entries before `start`
 are left as `null` so the caller can fill them manually (e.g., a "Default" label at index 0).

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **OnGUI(Rect, SerializedProperty, GUIContent)**

```csharp
public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
```

#### Parameters

`position` Rect<br>

`property` SerializedProperty<br>

`label` GUIContent<br>
