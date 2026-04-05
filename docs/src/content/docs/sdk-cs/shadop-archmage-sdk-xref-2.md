---
title: 'XRef<V, T>'
---

Namespace: Shadop.Archmage.Sdk

Represents a cross-table reference using a config ID and a resolved object reference.

```csharp
public struct XRef<V, T>
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [XRef<V, T>](../shadop-archmage-sdk-xref-2/)<br>

**Remarks:**

XRef is a two-part structure: it stores both the config ID (serialized to JSON)
 and the resolved reference (set during the binding phase). This enables lazy resolution of references
 after all data is loaded.

## Properties

### **CfgId**

Config ID serialized to JSON; key for lookup during binding phase.

```csharp
public V CfgId { get; set; }
```

#### Property Value

V<br>

### **Ref**

Resolved object (not serialized). Populated by Atlas.BindRefs(); may be null if unresolved.

```csharp
public T Ref { get; set; }
```

#### Property Value

T<br>

## Constructors

### **XRef(V, T)**

```csharp
XRef(V cfgId, T refValue)
```

#### Parameters

`cfgId` V<br>

`refValue` T<br>
