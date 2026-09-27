---
title: 'UnityAddressablesGreedyFS'
description: 'IFS implementation that loads files via Unity Addressables, reading all files in each encountered asset bundle in one go.'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to load files via Unity Addressables,
reading all files in each encountered asset bundle in one go.

```csharp
public class UnityAddressablesGreedyFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityAddressablesGreedyFS](.)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)

:::caution
Only asynchronous loading is supported. Calling `ReadAllBytes` will throw.
Use `LoadAtlasAsync` when using this FS implementation, and call it from the Unity main thread.
:::

On WebGL, [UnityAddressablesFS](../shadop-archmage-sdk-unityaddressablesfs/) takes about one frame per file,
because Unity completes at most one asynchronous asset load per frame. This class instead reads every file in an
asset bundle synchronously the first time a file in that bundle is requested, and keeps the other files in memory
until they are requested.

:::tip
This class reads every file in the bundle, so a bundle that also holds other files costs extra time and memory.
Keep configs in asset bundles of their own. Files that are never requested stay in memory until the instance is
garbage collected.
:::

## Constructors

### **UnityAddressablesGreedyFS()**

```csharp
public UnityAddressablesGreedyFS()
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

Always returns true. For a missing file, `ReadAllBytesAsync` throws `FileNotFoundException`.

```csharp
public bool FileExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DirectoryExists(String)**

Always returns true. Addressables has no directory concept.

```csharp
public bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
