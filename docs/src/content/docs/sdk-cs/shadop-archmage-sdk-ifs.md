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
 [Archmage.LoadAtlasAsync(String, String, IAtlas, AtlasOptions, Boolean, IProgress<AtlasLoadEvent>, CancellationToken)](../shadop-archmage-sdk-archmage/#loadatlasasyncstring-string-iatlas-atlasoptions-boolean-iprogressatlasloadevent-cancellationtoken) rely on this to skip missing override files.

If [IFS.MainThreadOnly](../shadop-archmage-sdk-ifs/#mainthreadonly) is true for the main IFS or for any override IFS, loading must start on
 the main thread. LoadAtlas and LoadAtlasAsync then call the methods of every IFS on the calling thread.

Otherwise, if MainThreadOnly is false for every IFS, LoadAtlas and LoadAtlasAsync read the files of atlas
 items with [IFS.ReadAllBytes(String)](../shadop-archmage-sdk-ifs/#readallbytesstring), and may call the methods on thread pool threads.

## Properties

### **MainThreadOnly**

Gets whether the methods of this file system can be called only on the main thread.

```csharp
public abstract bool MainThreadOnly { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

**Remarks:**

Return true if the methods work only on the main thread, such as those that call Unity APIs. Return
 false if they can be called on any thread.

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
