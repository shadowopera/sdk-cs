---
title: 'IFS'
---

Namespace: Shadop.Archmage.Sdk

File system abstraction for Archmage configuration loading.

```csharp
public interface IFS
```


**Remarks:**

Implementations read local data only. Downloading remote content is the caller's
 responsibility and must be done before loading.

When a file does not exist, [IFS.ReadAllBytes(String)](../shadop-archmage-sdk-ifs/#readallbytesstring) and [IFS.ReadAllBytesAsync(String, CancellationToken)](../shadop-archmage-sdk-ifs/#readallbytesasyncstring-cancellationtoken)
 throw [FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception). The loader relies on this to skip missing
 override files.

During asynchronous loading, any method may be called from a thread pool thread.

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
false if the file is known not to exist; otherwise, true.

**Remarks:**

May return true for a missing file when an exact check is expensive; reading that file then
 throws [FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception). Must not return false for an existing file.

### **DirectoryExists(String)**

Determines whether the given path refers to an existing directory.

```csharp
bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The path to test.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
false if the directory is known not to exist; otherwise, true.

**Remarks:**

Used to validate override roots before loading. May return true when the underlying storage has
 no directory concept or an exact check is expensive. Must not return false for an existing directory.

### **PrepareAsync(IReadOnlyCollection<String>, CancellationToken)**

Prepares for the [IFS.FileExists(String)](../shadop-archmage-sdk-ifs/#fileexistsstring) calls of an asynchronous load.

```csharp
Task PrepareAsync(IReadOnlyCollection<string> paths, CancellationToken cancellationToken)
```

#### Parameters

`paths` [IReadOnlyCollection<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlycollection-1)<br>
The override file paths, as they will be passed to [IFS.FileExists(String)](../shadop-archmage-sdk-ifs/#fileexistsstring).

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The token to monitor for cancellation requests.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
A task that represents the asynchronous operation.

**Remarks:**

[Archmage.LoadAtlasAsync(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>, CancellationToken)](../shadop-archmage-sdk-archmage/#loadatlasasyncstring-string-iatlas-atlasoptions-iprogressatlasloadevent-cancellationtoken) calls this once per load, before any item is loaded, with every
 override file path it will pass to [IFS.FileExists(String)](../shadop-archmage-sdk-ifs/#fileexistsstring) on this instance. It is not called by
 [Archmage.LoadAtlas(String, String, IAtlas, AtlasOptions, IProgress<AtlasLoadEvent>)](../shadop-archmage-sdk-archmage/#loadatlasstring-string-iatlas-atlasoptions-iprogressatlasloadevent) or when there are no override files. Implementations that check file
 existence cheaply can return a completed task.
