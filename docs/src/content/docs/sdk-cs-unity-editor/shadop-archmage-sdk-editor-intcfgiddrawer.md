---
title: 'IntCfgIdDrawer<TId, TValue>'
description: 'Abstract PropertyDrawer base for config ID fields where TValue is the unmanaged numeric type underlying TId.'
---

Namespace: Shadop.Archmage.Sdk.Editor

Generic base for config ID property drawers. `TId` is the config ID struct type; `TValue` is the unmanaged numeric type of its underlying raw value. Manages the static ID-value and display-name arrays; subclasses populate them by calling `Initialize`.

```csharp
public abstract class IntCfgIdDrawer<TId, TValue> : CfgIdDrawer<TValue>
    where TValue : unmanaged, IComparable<TValue>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PropertyDrawer](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) → [CfgIdDrawer\<TValue\>](../shadop-archmage-sdk-editor-cfgiddrawer/) → [IntCfgIdDrawer\<TId, TValue\>](.)

## Methods

### **Initialize(String, ICollection\<TId\>, Func\<TId, TValue\>, Func\<TValue, String\>)**

Populates the static ID-value and display-name arrays from a table's ID collection. Index 0 is reserved for a "0 (Default)" entry; remaining entries are sorted ascending.

```csharp
protected static void Initialize(string header, ICollection<TId> ids, Func<TId, TValue> id2value, Func<TValue, string> format)
```

#### Parameters

`header` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The header text displayed at the top of the dropdown window.

`ids` [ICollection\<TId\>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1)<br>
The ID collection of the config table.

`id2value` [Func\<TId, TValue\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Extracts the raw value from a config ID (e.g. `id => id.Value`).

`format` [Func\<TValue, String\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Formats a raw value into its display string.
