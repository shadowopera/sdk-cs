---
title: 'GodotFileAccessFS'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to read files with Godot's `FileAccess`. Paths can
 be `res://` paths, `user://` paths or operating system paths, and files in mounted resource packs
 can be read too.

```csharp
public class GodotFileAccessFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [GodotFileAccessFS](../shadop-archmage-sdk-godotfileaccessfs/)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)<br>

**Remarks:**

If you mount resource packs with `ProjectSettings.LoadResourcePack`, mount them before loading starts.

## Properties

### **MainThreadOnly**

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

```csharp
public Task<Byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task<Byte[]>](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

**Remarks:**

Godot has no asynchronous file read, so this method calls [GodotFileAccessFS.ReadAllBytes(String)](../shadop-archmage-sdk-godotfileaccessfs/#readallbytesstring) on a
 thread pool thread.

### **FileExists(String)**

```csharp
public bool FileExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DirectoryExists(String)**

```csharp
public bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

**Remarks:**

Always returns true for `res://` paths, because `DirAccess` does not report them reliably after
 the project is exported or a resource pack is mounted.
