---
title: 'AtlasConstants'
---

Namespace: Shadop.Archmage.Sdk

Constants used by the Atlas system.

```csharp
public static class AtlasConstants
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AtlasConstants](../shadop-archmage-sdk-atlasconstants/)<br>

## Fields

### **VariantMappingDefaultKey**

Key for the default file in a MappingVariant group.

```csharp
public static string VariantMappingDefaultKey;
```

### **MappingUnique**

Indicates a one-to-one mapping between a key and a file.

```csharp
public static string MappingUnique;
```

### **MappingVariant**

Indicates that a key maps to multiple file variants, only one of which is loaded.

```csharp
public static string MappingVariant;
```

### **MappingMany**

Indicates that a key maps to multiple files loaded separately and merged into one.

```csharp
public static string MappingMany;
```
