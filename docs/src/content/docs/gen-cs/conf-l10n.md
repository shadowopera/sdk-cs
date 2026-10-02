---
title: 'L10n'
description: 'A localization key that resolves to translated text via the active I18n instance.'
---

A localization key that resolves to translated text via the active I18n instance.

```csharp
public readonly struct L10n
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [L10n](.)

## Fields

### **Empty**

```csharp
public static L10n Empty
```

#### Field Value

[L10n](.)<br>

### **GetI18n**

Returns the active I18n instance used by L10n. This is a global setting and must be set before calling L10n.GetText or L10n.Text.

```csharp
public static Func<I18n> GetI18n
```

#### Field Value

[Func\<I18n\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<br>

### **GetPreferredLanguage**

Returns the player's preferred language code used by L10n. This is a global setting and must be set before calling L10n.Text.

```csharp
public static Func<string> GetPreferredLanguage
```

#### Field Value

[Func\<String\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<br>

## Constructors

### **L10n(String)**

```csharp
public L10n(string key)
```

#### Parameters

`key` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Properties

### **Text**

Returns the translation in the player's preferred language, falling back to the default language if no translation is found, and finally to the key string if neither language has a translation.

```csharp
public string Text { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **GetText(String, String&)**

Returns true on success, with the translated text in `text`; otherwise false. An empty key succeeds with an empty string.

```csharp
public bool GetText(string lang, String& text)
```

#### Parameters

`lang` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`text` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ToString()**

```csharp
public override string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
