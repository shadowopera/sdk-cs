---
title: 'UnityAddressablesFS'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to load files via Unity Addressables.
 Only `LoadAtlasAsync` is supported, and it must be called from the main thread.

```csharp
public class UnityAddressablesFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityAddressablesFS](../shadop-archmage-sdk-unityaddressablesfs/)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)<br>

**Remarks:**

Each asset is released as soon as it is read, so an asset bundle may be unloaded and
 reloaded during a single load. Optionally, holding a handle to an asset in the bundle until
 loading finishes keeps the bundle from being unloaded. An empty placeholder file in the
 bundle works well for this purpose:

```csharp
var pin = Addressables.LoadAssetAsync<TextAsset>("Assets/Configs/placeholder.txt");
await pin.Task;
try
{
    await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, atlas, options);
}
finally
{
    Addressables.Release(pin);
}
```

## Properties

### **MainThreadOnly**

```csharp
public bool MainThreadOnly { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **UnityAddressablesFS()**

```csharp
public UnityAddressablesFS()
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

Always returns true. A missing file is reported by the read instead.

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
