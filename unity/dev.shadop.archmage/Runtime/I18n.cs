#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
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

        /// <summary>
        /// Gets the fallback language code.
        /// </summary>
        public string Fallback() => _fallbackLanguage;

        /// <summary>
        /// Returns the internal translation structure: language → (key → text).
        /// Modifications to the returned dictionary will affect future lookups.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> AllTexts() => _texts;

        /// <summary>
        /// Merges translation key-value pairs for a specific language.
        /// Creates the language entry if it doesn't exist; overwrites existing keys.
        /// </summary>
        /// <param name="texts">Dictionary of translation key-value pairs.</param>
        /// <param name="language">The language code to merge into.</param>
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
        /// Parses JSON bytes (flat object) and merges translations for the specified language.
        /// </summary>
        /// <param name="data">UTF-8 encoded JSON bytes of a flat object.</param>
        /// <param name="language">The language code to merge into.</param>
        /// <exception cref="JsonException">Thrown if JSON parsing fails.</exception>
        public void MergeL10nData(byte[] data, string language)
        {
            var texts = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(data));
            if (texts is not null)
            {
                MergeTexts(texts, language);
            }
        }

        /// <summary>
        /// Loads a localization JSON file and merges translations for the specified language.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (flat object with string keys/values).</param>
        /// <param name="language">The language code to merge into.</param>
        /// <param name="fs">Optional file system abstraction; defaults to <see cref="File.ReadAllBytes"/> if null.</param>
        /// <exception cref="ArchmageException">Thrown if reading the file or parsing JSON fails.</exception>
        public void MergeL10nFile(string filePath, string language, IFS? fs = null)
        {
            fs ??= new DefaultFS();
            var data = fs.ReadAllBytes(filePath);
            try
            {
                MergeL10nData(data, language);
            }
            catch (Exception ex)
            {
                throw new ArchmageException($"Failed to merge l10n file \"{filePath}\". language: {language}", ex);
            }
        }

        /// <summary>
        /// Asynchronously loads a localization JSON file and merges translations for the specified language.
        /// </summary>
        /// <param name="filePath">Path to the JSON file (flat object with string keys/values).</param>
        /// <param name="language">The language code to merge into.</param>
        /// <param name="fs">Optional file system abstraction; defaults to <see cref="File.ReadAllBytesAsync"/> if null.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <exception cref="ArchmageException">Thrown if reading the file or parsing JSON fails.</exception>
        public async Task MergeL10nFileAsync(string filePath, string language, IFS? fs = null, CancellationToken cancellationToken = default)
        {
            fs ??= new DefaultFS();
            var data = await fs.ReadAllBytesAsync(filePath, cancellationToken);
            try
            {
                MergeL10nData(data, language);
            }
            catch (Exception ex)
            {
                throw new ArchmageException($"Failed to merge l10n file \"{filePath}\". language: {language}", ex);
            }
        }

        /// <summary>
        /// Attempts to retrieve text with fallback.
        /// Tries the requested language first, then falls back to the default language.
        /// </summary>
        /// <param name="key">The translation key.</param>
        /// <param name="language">The preferred language code.</param>
        /// <param name="text">The retrieved text, or null if not found.</param>
        /// <returns>True if the key was found in either the requested or fallback language; otherwise, false.</returns>
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
        /// Retrieves text with fallback, throwing an exception if not found.
        /// Tries the requested language first, then falls back to the default language.
        /// </summary>
        /// <param name="key">The translation key.</param>
        /// <param name="language">The preferred language code.</param>
        /// <returns>The translation string.</returns>
        /// <exception cref="ArchmageException">Thrown if the key is not found in either the requested or fallback language.</exception>
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

            throw new ArchmageException($"Localization key not found: {key}.");
        }
    }
}
