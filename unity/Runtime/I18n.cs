using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Manages internationalized text with automatic fallback to a default language.
    /// </summary>
    public class I18n
    {
        readonly string _fallbackLanguage;
        readonly Dictionary<string, Dictionary<string, string>> _texts;

        /// <summary>
        /// Creates a new I18n instance.
        /// </summary>
        /// <param name="fallbackLanguage">The default language to use when a key is not found in the requested language.</param>
        public I18n(string fallbackLanguage)
        {
            _fallbackLanguage = fallbackLanguage ?? throw new ArgumentNullException(nameof(fallbackLanguage));
            _texts = new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>
        /// Returns the fallback language identifier.
        /// </summary>
        public string Fallback() => _fallbackLanguage;

        /// <summary>
        /// Returns all loaded translations.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> AllTexts() => _texts;

        /// <summary>
        /// Adds or updates translations for the specified language.
        /// </summary>
        /// <param name="texts">The translation key-value pairs to merge.</param>
        /// <param name="language">The language identifier.</param>
        public void MergeTexts(Dictionary<string, string> texts, string language)
        {
            if (!_texts.TryGetValue(language, out var store))
            {
                store = new Dictionary<string, string>();
                _texts[language] = store;
            }

            foreach (var kvp in texts)
            {
                store[kvp.Key] = kvp.Value;
            }
        }

        /// <summary>
        /// Parses JSON translation data and merges it for the specified language.
        /// </summary>
        /// <param name="data">JSON bytes containing string key-value pairs.</param>
        /// <param name="language">The language identifier.</param>
        public void MergeL10nData(byte[] data, string language)
        {
            var texts = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(data))
                ?? throw new ArchmageException("failed to parse localization data");
            MergeTexts(texts, language);
        }

        /// <summary>
        /// Loads and merges a localization file into the specified language.
        /// The file should be a JSON object with string keys and values.
        /// </summary>
        /// <param name="filePath">Path to the JSON localization file.</param>
        /// <param name="language">The language identifier (e.g., "en", "zh-CN").</param>
        public void MergeL10nFile(string filePath, string language)
        {
            byte[] data;
            try
            {
                data = File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                throw new ArchmageException($"localization file not found: {filePath}", ex);
            }

            MergeL10nData(data, language);
        }

        /// <summary>
        /// Tries to get the localized text for a key. If the key is not found in the requested language,
        /// falls back to the default language. Returns false if the key is not found in either language.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="language">The requested language.</param>
        /// <param name="text">The localized text, or null if not found.</param>
        /// <returns>True if the key was found; otherwise false.</returns>
        public bool GetText(string key, string language, out string? text)
        {
            // Try requested language
            if (_texts.TryGetValue(language, out var languageTexts) &&
                languageTexts.TryGetValue(key, out text))
            {
                return true;
            }

            // Fallback to default language
            if (_texts.TryGetValue(_fallbackLanguage, out var fallbackTexts) &&
                fallbackTexts.TryGetValue(key, out text))
            {
                return true;
            }

            text = null;
            return false;
        }

        /// <summary>
        /// Gets the localized text for a key. If the key is not found in the requested language,
        /// falls back to the default language. Throws if the key is not found in either language.
        /// </summary>
        /// <param name="key">The localization key.</param>
        /// <param name="language">The requested language.</param>
        /// <returns>The localized text.</returns>
        public string Text(string key, string language)
        {
            // Try requested language
            if (_texts.TryGetValue(language, out var languageTexts) &&
                languageTexts.TryGetValue(key, out var text))
            {
                return text;
            }

            // Fallback to default language
            if (_texts.TryGetValue(_fallbackLanguage, out var fallbackTexts) &&
                fallbackTexts.TryGetValue(key, out var fallbackText))
            {
                return fallbackText;
            }

            throw new ArchmageException($"localization key not found: {key}");
        }
    }
}
