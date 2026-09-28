---
title: 'GodotAtlasLogger'
description: 'IAtlasLogger adapter that pipes Archmage log output to the Godot output.'
---

Namespace: Shadop.Archmage.Sdk

Simple logger adapter to pipe Archmage internal output to the Godot output with `GD.Print`, which can be
called on any thread.

```csharp
public class GodotAtlasLogger : IAtlasLogger
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotAtlasLogger](.)<br>
Implements [IAtlasLogger](../../sdk-cs/shadop-archmage-sdk-iatlaslogger/)

## Constructors

### **GodotAtlasLogger()**

```csharp
public GodotAtlasLogger()
```

## Methods

### **Info(String)**

```csharp
public void Info(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
