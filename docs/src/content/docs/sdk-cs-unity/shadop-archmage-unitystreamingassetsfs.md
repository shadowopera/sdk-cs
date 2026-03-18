---
title: 'UnityStreamingAssetsFS'
---

Namespace: Shadop.Archmage

Implements the IFS interface to load files from Unity StreamingAssets via UnityWebRequest.
Paths are resolved relative to Application.streamingAssetsPath.
Only asynchronous loading is supported.

```csharp
public class UnityStreamingAssetsFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityStreamingAssetsFS](./shadop.archmage.unitystreamingassetsfs.md)<br>
Implements [IFS](../sdk-cs/shadop.archmage.ifs.md)<br>
Attributes [NullableContextAttribute](../sdk-cs/system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](../sdk-cs/system.runtime.compilerservices.nullableattribute.md)

## Constructors

### **UnityStreamingAssetsFS()**

```csharp
public UnityStreamingAssetsFS()
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
