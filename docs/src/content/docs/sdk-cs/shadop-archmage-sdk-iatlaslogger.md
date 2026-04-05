---
title: 'IAtlasLogger'
---

Namespace: Shadop.Archmage.Sdk

Interface for logging during Atlas loading and operations.

```csharp
public interface IAtlasLogger
```


**Remarks:**

Implement this interface to provide custom logging behavior for the Atlas loading system.
 The default implementation logs to the console with standardized prefixes.

## Methods

### **Info(String)**

```csharp
void Info(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
