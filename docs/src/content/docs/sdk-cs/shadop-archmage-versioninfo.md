---
title: 'VersionInfo'
---

Namespace: Shadop.Archmage

Represents VCS version metadata.

```csharp
public class VersionInfo
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [VersionInfo](./shadop.archmage.versioninfo.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

## Properties

### **Workspace**

The workspace name.

```csharp
public string Workspace { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Tags**

Tags associated with this version.

```csharp
public List<string> Tags { get; set; }
```

#### Property Value

[List<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Branch**

The source control branch name.

```csharp
public string Branch { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ID**

The full commit ID.

```csharp
public string ID { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ShortID**

The abbreviated commit ID.

```csharp
public string ShortID { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Timestamp**

The commit timestamp.

```csharp
public DateTimeOffset Timestamp { get; set; }
```

#### Property Value

[DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset)<br>

### **Message**

The commit message.

```csharp
public string Message { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Author**

The commit author.

```csharp
public string Author { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Status**

Working tree status entries.

```csharp
public List<string> Status { get; set; }
```

#### Property Value

[List<String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Extra**

Additional metadata.

```csharp
public Dictionary<string, object> Extra { get; set; }
```

#### Property Value

[Dictionary<String, Object>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

## Constructors

### **VersionInfo()**

```csharp
public VersionInfo()
```
