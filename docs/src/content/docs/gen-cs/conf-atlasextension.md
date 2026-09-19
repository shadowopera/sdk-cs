---
title: 'AtlasExtension'
description: 'User-editable extension point for adding custom logic after all config data has been loaded.'
---

User-editable extension point for adding custom logic after all config data has been loaded.

```csharp
public partial class AtlasExtension
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AtlasExtension](.)

## Remarks

This file is generated once and **will not be regenerated**, so it is safe to edit. Use it to add custom computed fields, config entry indexes, or any post-load initialization logic.

An instance is created automatically by [ConfigAtlas](../conf-configatlas/) and is accessible via [ConfigAtlas.Extension](../conf-configatlas/#extension).

## Methods

### **OnLoaded(ConfigAtlas)**

Called by [ConfigAtlas.OnLoaded](../conf-configatlas/#onloaded) after all config tables have been deserialized and cross-table references bound. Place your custom initialization logic here.

```csharp
public void OnLoaded(ConfigAtlas atlas)
```

#### Parameters

`atlas` [ConfigAtlas](../conf-configatlas/)<br>
The fully loaded atlas instance.
