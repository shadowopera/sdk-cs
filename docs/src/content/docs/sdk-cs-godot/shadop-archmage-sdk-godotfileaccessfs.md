---
title: 'GodotFileAccessFS'
description: 'IFS implementation that reads files with Godot''s FileAccess.'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to read files with Godot's `FileAccess`. Paths can be `res://` paths,
`user://` paths or operating system paths, and files in mounted resource packs can be read too.

```csharp
public class GodotFileAccessFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotFileAccessFS](.)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)

:::note
The methods can be called on any thread. Call `ProjectSettings.LoadResourcePack` before loading starts:
mounting a resource pack while files are being read is not supported.
:::

## Properties

### **MainThreadOnly**

Always `false`.

```csharp
public bool MainThreadOnly { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **GodotFileAccessFS()**

```csharp
public GodotFileAccessFS()
```

## Methods

### **ReadAllBytes(String)**

```csharp
public Byte[] ReadAllBytes(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **ReadAllBytesAsync(String, CancellationToken)**

Godot has no asynchronous file read, so this method calls [ReadAllBytes(String)](#readallbytesstring) on a thread pool
thread.

```csharp
public Task<Byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;Byte[]&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **FileExists(String)**

```csharp
public bool FileExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DirectoryExists(String)**

Always returns true for `res://` paths, because `DirAccess` does not report them reliably after
the project is exported or a resource pack is mounted.

```csharp
public bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
