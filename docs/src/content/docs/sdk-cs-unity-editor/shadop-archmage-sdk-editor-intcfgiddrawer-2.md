---
title: 'IntCfgIdDrawer<TId, TValue>'
---

Namespace: Shadop.Archmage.Sdk.Editor

Generic base for config ID property drawers. `TId` is the config ID struct type;
 `TValue` is the unmanaged numeric type of its underlying raw value.
 Manages the static ID-value and display-name arrays; subclasses populate them by calling `Initialize`.

```csharp
public abstract class IntCfgIdDrawer<TId, TValue> : CfgIdDrawer<TValue>
```

#### Type Parameters

`TId`<br>
The config ID struct type (e.g. `HeroCfgId`).

`TValue`<br>
The underlying unmanaged value type (e.g. `long`).

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → GUIDrawer → PropertyDrawer → CfgIdDrawer<TValue> → [IntCfgIdDrawer<TId, TValue>](../shadop-archmage-sdk-editor-intcfgiddrawer-2/)<br>

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

### **IntCfgIdDrawer()**

```csharp
protected IntCfgIdDrawer()
```

## Methods

### **GetHeader()**

```csharp
protected string GetHeader()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetIdValues()**

```csharp
protected TValue[] GetIdValues()
```

#### Returns

TValue[]<br>

### **GetDisplayNames()**

```csharp
protected String[] GetDisplayNames()
```

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Initialize(String, ICollection<TId>, Func<TId, TValue>, Func<TValue, String>)**

Populates the static ID-value and display-name arrays from a table's ID collection.
 Index 0 is reserved for a `0 (Default)` entry; remaining entries are sorted ascending.

```csharp
protected static void Initialize(string header, ICollection<TId> ids, Func<TId, TValue> id2Value, Func<TValue, string> format)
```

#### Parameters

`header` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The header text displayed at the top of the dropdown window.

`ids` ICollection<TId><br>
The ID collection of the config table.

`id2Value` Func<TId, TValue><br>
Extracts the raw value from a config ID (e.g. `id => id.Value`).

`format` Func<TValue, String><br>
Formats a raw value into its display string.
