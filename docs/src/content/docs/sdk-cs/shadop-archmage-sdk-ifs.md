---
title: 'IFS'
---

Namespace: Shadop.Archmage.Sdk

File system abstraction for Archmage configuration loading.

```csharp
public interface IFS
```


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
true if the caller has the required permissions and path contains the name of an existing file; otherwise, false.

### **DirectoryExists(String)**

Determines whether the given path refers to an existing directory on disk.

```csharp
bool DirectoryExists(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The path to test.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified directory exists.
