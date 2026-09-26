---
title: 'UnityAddressablesFS'
description: 'IFS implementation that loads files via Unity Addressables.'
---

Namespace: Shadop.Archmage.Sdk

Implements the [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/) interface to load files via Unity Addressables.

```csharp
public class UnityAddressablesFS : IFS
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [UnityAddressablesFS](.)<br>
Implements [IFS](../../sdk-cs/shadop-archmage-sdk-ifs/)

:::caution
Only asynchronous loading is supported. Calling `ReadAllBytes` will throw.
Use `LoadAtlasAsync` when using this FS implementation, and call it from the Unity main thread.
Content in remote groups must be downloaded before loading.
:::

:::tip
Each asset is released as soon as it has been read, so an asset bundle may be unloaded and loaded
again during a single load. Optionally, holding a handle to an asset in the bundle until loading
finishes keeps it loaded. An empty placeholder file in the bundle works well for this:

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
:::

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
