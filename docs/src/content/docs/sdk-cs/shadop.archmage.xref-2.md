---
title: 'XRef<V, T>'
---

Namespace: Shadop.Archmage

Represents a cross-table reference using a raw identifier and a resolved object reference.

```csharp
public struct XRef<V, T>
```

#### Type Parameters

`V`<br>

`T`<br>

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [XRef<V, T>](./shadop.archmage.xref-2.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md), JsonConverterAttribute

**Remarks:**

XRef is a two-part structure: it stores both the raw identifier (serialized to JSON)
 and the resolved reference (set during the binding phase). This enables lazy resolution of references
 after all data is loaded.

Important: V should be int, long, or string (the identifier type).
 Using other types may cause issues during serialization or reference binding.

## Properties

### **RawValue**

Raw identifier serialized to JSON; key for lookup during binding phase.

```csharp
public V RawValue { get; set; }
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
XRef(V rawValue, T refValue)
```

#### Parameters

`rawValue` V<br>

`refValue` T<br>
