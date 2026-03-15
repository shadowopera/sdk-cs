---
title: 'Duration'
---

Namespace: Shadop.Archmage

Represents a duration as a signed number of nanoseconds.

```csharp
public struct Duration
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Duration](./shadop.archmage.duration.md)<br>
Implements [IEquatable<Duration>](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1), [IComparable<Duration>](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1)<br>
Attributes [IsReadOnlyAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isreadonlyattribute), JsonConverterAttribute

**Remarks:**

Duration provides methods to retrieve duration in various units (nanoseconds, microseconds, milliseconds, seconds, minutes, hours)
 and supports arithmetic operations, comparisons, and JSON serialization.

The struct uses nanosecond precision internally and includes static readonly constants for common durations.
 The ToString method formats durations in a human-readable format like "1h30m5s".

Duration serializes to and from a compact JSON array format for efficient storage.

## Fields

### **Zero**

```csharp
public static Duration Zero;
```

### **Nanosecond**

```csharp
public static Duration Nanosecond;
```

### **Microsecond**

```csharp
public static Duration Microsecond;
```

### **Millisecond**

```csharp
public static Duration Millisecond;
```

### **Second**

```csharp
public static Duration Second;
```

### **Minute**

```csharp
public static Duration Minute;
```

### **Hour**

```csharp
public static Duration Hour;
```

## Constructors

### **Duration(Int64)**

```csharp
Duration(long nanoseconds)
```

#### Parameters

`nanoseconds` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

## Methods

### **Abs()**

Returns the absolute value of the duration.

```csharp
Duration Abs()
```

#### Returns

[Duration](./shadop.archmage.duration.md)<br>

### **Hours()**

Returns floating-point duration in hours (may lose precision).

```csharp
double Hours()
```

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

### **Minutes()**

Returns floating-point duration in minutes (may lose precision).

```csharp
double Minutes()
```

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

### **Seconds()**

Returns floating-point duration in seconds (may lose precision).

```csharp
double Seconds()
```

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

### **Milliseconds()**

Returns integer millisecond count (truncates sub-millisecond precision).

```csharp
long Milliseconds()
```

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Microseconds()**

Returns integer microsecond count (truncates sub-microsecond precision).

```csharp
long Microseconds()
```

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Nanoseconds()**

```csharp
long Nanoseconds()
```

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br>

### **Truncate(Duration)**

Returns the result of truncating this duration toward zero to the nearest multiple of the specified duration.

```csharp
Duration Truncate(Duration m)
```

#### Parameters

`m` [Duration](./shadop.archmage.duration.md)<br>
The divisor duration. If zero or negative, this duration is returned unchanged.

#### Returns

[Duration](./shadop.archmage.duration.md)<br>
A Duration truncated to a multiple of m.

### **Round(Duration)**

Returns the result of rounding this duration to the nearest multiple of the specified duration.

```csharp
Duration Round(Duration m)
```

#### Parameters

`m` [Duration](./shadop.archmage.duration.md)<br>
The divisor duration. If zero or negative, this duration is returned unchanged.

#### Returns

[Duration](./shadop.archmage.duration.md)<br>
A Duration rounded to the nearest multiple of m.

### **ToString()**

Formats the duration as a human-readable string.

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A formatted duration string.

**Remarks:**

Examples: "1h30m5s", "500ms", "1us", "-2h15m".

### **ToTimeSpan()**

Converts to TimeSpan (100-nanosecond precision; may lose sub-tick precision).

```csharp
TimeSpan ToTimeSpan()
```

#### Returns

[TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

### **FromTimeSpan(TimeSpan)**

Creates Duration from TimeSpan.

```csharp
Duration FromTimeSpan(TimeSpan timeSpan)
```

#### Parameters

`timeSpan` [TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/system.timespan)<br>

#### Returns

[Duration](./shadop.archmage.duration.md)<br>

### **CompareTo(Duration)**

```csharp
int CompareTo(Duration other)
```

#### Parameters

`other` [Duration](./shadop.archmage.duration.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Duration)**

```csharp
bool Equals(Duration other)
```

#### Parameters

`other` [Duration](./shadop.archmage.duration.md)<br>

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
