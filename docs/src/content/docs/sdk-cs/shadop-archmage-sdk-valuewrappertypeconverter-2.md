---
title: 'ValueWrapperTypeConverter<T, V>'
---

Namespace: Shadop.Archmage.Sdk

Type converter for value wrapper structs that converts between  and [String](https://docs.microsoft.com/en-us/dotnet/api/system.string).

```csharp
public abstract class ValueWrapperTypeConverter<T, V> : System.ComponentModel.TypeConverter
```

#### Type Parameters

`T`<br>
The value wrapper struct type.

`V`<br>
The underlying value type.

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → TypeConverter → [ValueWrapperTypeConverter<T, V>](../shadop-archmage-sdk-valuewrappertypeconverter-2/)

## Constructors

### **ValueWrapperTypeConverter()**

```csharp
protected ValueWrapperTypeConverter()
```

## Methods

### **Create(V)**

```csharp
protected abstract T Create(V value)
```

#### Parameters

`value` V<br>

#### Returns

T<br>

### **CanConvertFrom(ITypeDescriptorContext, Type)**

```csharp
public bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
```

#### Parameters

`context` ITypeDescriptorContext<br>

`sourceType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ConvertFrom(ITypeDescriptorContext, CultureInfo, Object)**

```csharp
public object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
```

#### Parameters

`context` ITypeDescriptorContext<br>

`culture` [CultureInfo](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo)<br>

`value` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

### **CanConvertTo(ITypeDescriptorContext, Type)**

```csharp
public bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
```

#### Parameters

`context` ITypeDescriptorContext<br>

`destinationType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ConvertTo(ITypeDescriptorContext, CultureInfo, Object, Type)**

```csharp
public object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
```

#### Parameters

`context` ITypeDescriptorContext<br>

`culture` [CultureInfo](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo)<br>

`value` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

`destinationType` [Type](https://docs.microsoft.com/en-us/dotnet/api/system.type)<br>

#### Returns

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>
