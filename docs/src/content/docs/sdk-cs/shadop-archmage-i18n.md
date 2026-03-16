---
title: 'I18n'
---

Namespace: Shadop.Archmage

Manages multilingual text with automatic fallback to a default language.

```csharp
public class I18n
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [I18n](./shadop.archmage.i18n.md)<br>
Attributes [NullableContextAttribute](./system.runtime.compilerservices.nullablecontextattribute.md), [NullableAttribute](./system.runtime.compilerservices.nullableattribute.md)

**Remarks:**

I18n (internationalization) stores text for multiple languages and retrieves them on demand.
 When a text key is not available in the requested language, it automatically falls back to the configured fallback language.

Usage pattern:

1. Create an I18n instance with a fallback language
2. Load translations via MergeTexts, MergeL10nData, or MergeL10nFile
3. Retrieve translations via GetText or Text

## Constructors

### **I18n(String)**

Creates I18n with fallback language (used when key not found in requested language).

```csharp
public I18n(string fallbackLanguage)
```

#### Parameters

`fallbackLanguage` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown if fallbackLanguage is null.

## Methods

### **Fallback()**

Gets the fallback language code.

```csharp
public string Fallback()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **AllTexts()**

Returns the internal translation structure: language → (key → text).
 Modifications to the returned dictionary will affect future lookups.

```csharp
public Dictionary<string, Dictionary<string, string>> AllTexts()
```

#### Returns

[Dictionary<String, Dictionary<String, String>>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **MergeTexts(Dictionary<String, String>, String)**

Merges translation key-value pairs for a specific language.
 Creates the language entry if it doesn't exist; overwrites existing keys.

```csharp
public void MergeTexts(Dictionary<string, string> texts, string language)
```

#### Parameters

`texts` [Dictionary<String, String>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
Dictionary of translation key-value pairs.

`language` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The language code to merge into.

### **MergeL10nData(Byte[], String)**

Parses JSON bytes (flat object) and merges translations for the specified language.

```csharp
public void MergeL10nData(Byte[] data, string language)
```

#### Parameters

`data` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
UTF-8 encoded JSON bytes of a flat object.

`language` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The language code to merge into.

#### Exceptions

T:Newtonsoft.Json.JsonException<br>
Thrown if JSON parsing fails.

### **MergeL10nFile(String, String, IFS)**

Loads a localization JSON file and merges translations for the specified language.

```csharp
public void MergeL10nFile(string filePath, string language, IFS fs)
```

#### Parameters

`filePath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the JSON file (flat object with string keys/values).

`language` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The language code to merge into.

`fs` [IFS](./shadop.archmage.ifs.md)<br>
Optional file system abstraction; defaults to [File.ReadAllBytes(String)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readallbytes) if null.

#### Exceptions

[ArchmageException](./shadop.archmage.archmageexception.md)<br>
Thrown if reading the file or parsing JSON fails.

### **MergeL10nFileAsync(String, String, IFS, CancellationToken)**

Asynchronously loads a localization JSON file and merges translations for the specified language.

```csharp
public Task MergeL10nFileAsync(string filePath, string language, IFS fs, CancellationToken cancellationToken)
```

#### Parameters

`filePath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the JSON file (flat object with string keys/values).

`language` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The language code to merge into.

`fs` [IFS](./shadop.archmage.ifs.md)<br>
Optional file system abstraction; defaults to [File.ReadAllBytesAsync(String, CancellationToken)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readallbytesasync) if null.

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
Token to cancel the operation.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>

#### Exceptions

[ArchmageException](./shadop.archmage.archmageexception.md)<br>
Thrown if reading the file or parsing JSON fails.

### **GetText(String, String, String&)**

Attempts to retrieve text with fallback.
 Tries the requested language first, then falls back to the default language.

```csharp
public bool GetText(string key, string language, String& text)
```

#### Parameters

`key` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The translation key.

`language` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The preferred language code.

`text` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
The retrieved text, or null if not found.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if the key was found in either the requested or fallback language; otherwise, false.

### **Text(String, String)**

Retrieves text with fallback, throwing an exception if not found.
 Tries the requested language first, then falls back to the default language.

```csharp
public string Text(string key, string language)
```

#### Parameters

`key` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The translation key.

`language` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The preferred language code.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The translation string.

#### Exceptions

[ArchmageException](./shadop.archmage.archmageexception.md)<br>
Thrown if the key is not found in either the requested or fallback language.
