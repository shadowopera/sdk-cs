---
title: 'IFS'
---

Namespace: Shadop.Archmage.Sdk

File system abstraction for Archmage configuration loading.

```csharp
public interface IFS
```


**Remarks:**

When a file does not exist, [IFS.ReadAllBytes(String)](../shadop-archmage-sdk-ifs/#readallbytesstring) and [IFS.ReadAllBytesAsync(String, CancellationToken)](../shadop-archmage-sdk-ifs/#readallbytesasyncstring-cancellationtoken)
 must throw [FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception). [Archmage.LoadAtlas(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>)](../shadop-archmage-sdk-archmage/#loadatlasstring-string-iatlas-atlasoptions-iprogressatlasloadevent) and
 [Archmage.LoadAtlasAsync(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>, CancellationToken)](../shadop-archmage-sdk-archmage/#loadatlasasyncstring-string-iatlas-atlasoptions-iprogressatlasloadevent-cancellationtoken) rely on this to skip missing override files.

LoadAtlas and LoadAtlasAsync call these methods on the calling thread. If that thread has no
 [SynchronizationContext](https://docs.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext), LoadAtlasAsync may call them on thread pool threads instead.

## Methods

### **ReadAllBytes(String)**

Reads all bytes from the specified file.

```csharp
Byte[] ReadAllBytes(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The file path.

#### Returns

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
A byte array containing the contents of the file.

#### Exceptions

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>
The file does not exist.

### **ReadAllBytesAsync(String, CancellationToken)**

Asynchronously reads all bytes from the specified file.

```csharp
Task<Byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The file path.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The token to monitor for cancellation requests.

#### Returns

[Task<Byte[]>](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
A task that represents the asynchronous read operation, wrapping the file contents as a byte array.

#### Exceptions

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>
The file does not exist.

### **FileExists(String)**

Determines whether the specified file exists.

```csharp
bool FileExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The file to check.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if the file exists or the check was skipped; otherwise, false.

**Remarks:**

If checking is expensive, an implementation may skip the check and return true.

### **DirectoryExists(String)**

Determines whether the specified directory exists.

```csharp
bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The directory to check.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if the directory exists or the check was skipped; otherwise, false.

**Remarks:**

If the storage has no directories, or checking is expensive, an implementation may skip the check
 and return true.
