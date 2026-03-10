using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Manages multilingual text with automatic fallback to a default language.
    /// </summary>
    /// <remarks>
    /// <para>I18n (internationalization) stores text for multiple languages and retrieves them on demand.
    /// When a text key is not available in the requested language, it automatically falls back to the configured fallback language.</para>
    /// <para>Usage pattern:</para>
    /// <list type="number">
    /// <item><description>Create an I18n instance with a fallback language</description></item>
    /// <item><description>Load translations via MergeTexts, MergeL10nData, or MergeL10nFile</description></item>
    /// <item><description>Retrieve translations via GetText or Text</description></item>
    /// </list>
    /// </remarks>
    public class I18n
    {
        readonly string _fallbackLanguage;
        readonly Dictionary<string, Dictionary<string, string>> _texts;

        /// <summary>
        /// Creates I18n with fallback language (used when key not found in requested language).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if fallbackLanguage is null.</exception>
        public I18n(string fallbackLanguage)
        {
            _fallbackLanguage = fallbackLanguage ?? throw new ArgumentNullException(nameof(fallbackLanguage));
            _texts = new Dictionary<string, Dictionary<string, string>>();
        }

        public string Fallback() => _fallbackLanguage;

        /// <summary>
        /// Returns internal structure: language → (key → text). Modifications affect lookups.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> AllTexts() => _texts;

        /// <summary>
        /// Merges translation key-value pairs for language (creates if doesn't exist, overwrites existing keys).
        /// </summary>
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
        /// Parses JSON bytes (flat object) and merges for language.
        /// </summary>
        /// <exception cref="ArchmageException">Thrown if JSON parsing fails.</exception>
        public void MergeL10nData(byte[] data, string language)
        {
            var texts = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(data))
                ?? throw new ArchmageException("failed to parse localization data");
            MergeTexts(texts, language);
        }

        /// <summary>
        /// Loads localization JSON file and merges for language (flat object with string keys/values).
        /// </summary>
        /// <exception cref="ArchmageException">Thrown if file not found or JSON parsing fails.</exception>
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
        /// Retrieves text with fallback (tries requested, then fallback language).
        /// </summary>
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
        /// Retrieves text with fallback (tries requested, then fallback; throws if not found).
        /// </summary>
        /// <exception cref="ArchmageException">Thrown if key not found in either language.</exception>
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
