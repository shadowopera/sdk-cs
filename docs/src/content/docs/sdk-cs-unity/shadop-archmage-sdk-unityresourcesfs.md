---
title: 'UnityResourcesFS'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to load files via Unity Resources.
 Paths are resolved relative to any Resources folder; file extensions are stripped automatically.
 Loading must be started from the main thread.

```csharp
public class UnityResourcesFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityResourcesFS](../shadop-archmage-sdk-unityresourcesfs/)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)<br>

## Properties

### **MainThreadOnly**

```csharp
public bool MainThreadOnly { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

Always returns true. Resources can only check existence by loading the asset,
 so a missing file is reported by the read instead.

```csharp
public bool FileExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DirectoryExists(String)**

Always returns true. Resources has no directory concept.

```csharp
public bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
