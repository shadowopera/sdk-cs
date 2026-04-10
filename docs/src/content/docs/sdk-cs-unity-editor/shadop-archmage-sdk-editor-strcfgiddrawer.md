---
title: 'StrCfgIdDrawer<TId>'
description: 'Abstract PropertyDrawer base for config ID fields where the underlying raw value of TId is a string.'
---

Namespace: Shadop.Archmage.Sdk.Editor

Generic base for config ID property drawers. `TId` is the config ID struct type whose underlying raw value is a `string`. Manages the static ID-value and display-name arrays; subclasses populate them by calling `Initialize`.

```csharp
public abstract class StrCfgIdDrawer<TId> : CfgIdDrawer<string>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PropertyDrawer](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html) → [CfgIdDrawer\<String\>](../shadop-archmage-sdk-editor-cfgiddrawer/) → [StrCfgIdDrawer\<TId\>](.)

## Methods

### **Initialize(String, ICollection\<TId\>, Func\<TId, String\>, Func\<String, String\>)**

Populates the static ID-value and display-name arrays from a table's ID collection. Index 0 is reserved for a `"" (Default)` entry; remaining entries are sorted ascending.

```csharp
protected static void Initialize(string header, ICollection<TId> ids, Func<TId, string> id2value, Func<string, string> format)
```

#### Parameters

`header` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The header text displayed at the top of the dropdown window.

`ids` [ICollection\<TId\>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1)<br>
The ID collection of the config table.

`id2value` [Func\<TId, String\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Extracts the raw value from a config ID (e.g. `id => id.Value`).

`format` [Func\<String, String\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)<br>
Formats a raw value into its display string.
