---
title: 'StrCfgIdDrawer<TId>'
---

Namespace: Shadop.Archmage.Sdk.Editor

Generic base for config ID property drawers. `TId` is the config ID struct type
 whose underlying raw value is a [String](https://docs.microsoft.com/en-us/dotnet/api/system.string).
 Manages the static ID-value and display-name arrays; subclasses populate them by calling `Initialize`.

```csharp
public abstract class StrCfgIdDrawer<TId> : CfgIdDrawer<String>
```

#### Type Parameters

`TId`<br>
The config ID struct type (e.g. `RaceCfgId`).

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → GUIDrawer → PropertyDrawer → [CfgIdDrawer<String>](../shadop-archmage-sdk-editor-cfgiddrawer-1/) → [StrCfgIdDrawer<TId>](../shadop-archmage-sdk-editor-strcfgiddrawer-1/)<br>

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

### **StrCfgIdDrawer()**

```csharp
protected StrCfgIdDrawer()
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
protected String[] GetIdValues()
```

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetDisplayNames()**

```csharp
protected String[] GetDisplayNames()
```

#### Returns

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Initialize(String, ICollection<TId>, Func<TId, String>, Func<String, String>)**

Populates the static ID-value and display-name arrays from a table's ID collection.
 Index 0 is reserved for a `"" (Default)` entry; remaining entries are sorted ascending.

```csharp
protected static void Initialize(string header, ICollection<TId> ids, Func<TId, string> id2Value, Func<string, string> format)
```

#### Parameters

`header` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The header text displayed at the top of the dropdown window.

`ids` ICollection<TId><br>
The ID collection of the config table.

`id2Value` Func<TId, String><br>
Extracts the raw value from a config ID (e.g. `id => id.Value`).

`format` [Func<String, String>](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Formats a raw value into its display string.
