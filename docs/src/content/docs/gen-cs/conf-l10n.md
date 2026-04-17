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

Returns the active I18n instance. It must be set before calling L10n.GetText or L10n.Text.

```csharp
public static Func<I18n>? GetI18n
```

#### Field Value

[Func\<I18n\>](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<br>

### **GetPreferredLanguage**

Returns the player's current language tag. It must be set before calling L10n.Text.

```csharp
public static Func<string>? GetPreferredLanguage
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

Returns the translation for the player's preferred language, falling back to the default language if the key isn't found.

```csharp
public string Text { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **GetText(String, String&)**

Returns the translation for the given language. Returns true if the key is found, with the translated text in `text`; otherwise false.

```csharp
public bool GetText(string lang, String& text)
```

#### Parameters

`lang` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`text` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The translated text, or null if not found.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ToString()**

```csharp
public override string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
