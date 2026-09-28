---
title: 'UnityStreamingAssetsFS'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to load files from Unity StreamingAssets via UnityWebRequest.
 Paths are resolved relative to Application.streamingAssetsPath.
 Only `LoadAtlasAsync` is supported, and it must be called from the main thread.

```csharp
public class UnityStreamingAssetsFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityStreamingAssetsFS](../shadop-archmage-sdk-unitystreamingassetsfs/)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)<br>

**Remarks:**

On WebGL, StreamingAssets is deployed to the web server along with the build, so files are downloaded
 over HTTP. A 404 response is treated as a missing file. Any other error, such as a 403 response or a
 network failure, makes the load fail.

## Properties

### **MainThreadOnly**

```csharp
public bool MainThreadOnly { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

Checks the file system directly where StreamingAssets is a plain directory.
 Returns true on platforms where it is a URI (Android, WebGL).

```csharp
public bool FileExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DirectoryExists(String)**

Checks the file system directly where StreamingAssets is a plain directory.
 Returns true on platforms where it is a URI (Android, WebGL).

```csharp
public bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
