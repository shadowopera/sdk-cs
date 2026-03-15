---
title: 'UnityResourcesFS'
---

Namespace: Shadop.Archmage

Implements the IFS interface to load files via Unity Resources.
Paths are resolved relative to any Resources folder; file extensions are stripped automatically.
Synchronous loading must be called from the main thread.

```csharp
public class UnityResourcesFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityResourcesFS](./shadop.archmage.unityresourcesfs.md)<br>
Implements [IFS](../sdk-cs/shadop.archmage.ifs.md)<br>
Attributes [NullableContextAttribute](../sdk-cs/system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](../sdk-cs/system.runtime.compilerservices.nullableattribute.md)

## Constructors

### **UnityResourcesFS()**

```csharp
public UnityResourcesFS()
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
