---
title: 'Rgba'
---

Namespace: Shadop.Archmage.Sdk

Represents a color with red, green, blue, and alpha channels.

```csharp
public struct Rgba
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Rgba](../shadop-archmage-sdk-rgba/)<br>
Implements [IEquatable<Rgba>](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1), [IZero](../shadop-archmage-sdk-izero/)<br>

**Remarks:**

Serializes to JSON as `"#RRGGBBAA"`. Deserializes from `"#RRGGBB"`,
 `"#RRGGBBAA"`, an empty string, or `null`.
 The zero value (all channels 0) serializes as an empty string.

## Properties

### **R**

```csharp
public byte R { get; }
```

#### Property Value

[Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **G**

```csharp
public byte G { get; }
```

#### Property Value

[Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **B**

```csharp
public byte B { get; }
```

#### Property Value

[Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **A**

```csharp
public byte A { get; }
```

#### Property Value

[Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### **IsZero**

Returns true if all channels are zero.

```csharp
public bool IsZero { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **Rgba(Byte, Byte, Byte, Byte)**

```csharp
Rgba(byte r, byte g, byte b, byte a)
```

#### Parameters

`r` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

`g` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

`b` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

`a` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

## Methods

### **Parse(String)**

Parses a `"#RRGGBB"` or `"#RRGGBBAA"` hex string.
 An empty or null string returns the zero value.

```csharp
Rgba Parse(string s)
```

#### Parameters

`s` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Rgba](../shadop-archmage-sdk-rgba/)<br>

#### Exceptions

[FormatException](https://docs.microsoft.com/en-us/dotnet/api/system.formatexception)<br>
Thrown when the string format is invalid.

### **TryParse(String, Rgba&)**

Tries to parse a `"#RRGGBB"` or `"#RRGGBBAA"` hex string.
 An empty or null string sets `result` to the zero value and returns true.

```csharp
bool TryParse(string s, Rgba& result)
```

#### Parameters

`s` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`result` [Rgba&](../shadop-archmage-sdk-rgba&/)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ToString()**

Returns the color as `"#RRGGBBAA"`.

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Equals(Rgba)**

```csharp
bool Equals(Rgba other)
```

#### Parameters

`other` [Rgba](../shadop-archmage-sdk-rgba/)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
