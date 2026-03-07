using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shadop.Archmage;
using Xunit;

namespace Shadop.Archmage.Tests
{
    public class I18nTests
    {
        [Fact]
        public void TestNewI18n()
        {
            var langs = new[] { "en", "zh", "und" };
            foreach (var lang in langs)
            {
                var i18n = new I18n(lang);
                Assert.Equal(lang, i18n.Fallback());
                Assert.NotNull(i18n.AllTexts());
                Assert.Empty(i18n.AllTexts());
            }
        }

        class MergeTextsTrial
        {
            public string subject { get; set; } = "";
            public Dictionary<string, string>? initialTexts { get; set; }
            public Dictionary<string, string>? mergeTexts { get; set; }
            public string lang { get; set; } = "";
            public Dictionary<string, Dictionary<string, string>> expected { get; set; } = new();
        }

        [Fact]
        public void TestI18n_MergeTexts()
        {
            var dataset = new MergeTextsTrial[]
            {
                new MergeTextsTrial {
                    subject = "merge into empty",
                    initialTexts = null,
                    mergeTexts = new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } },
                    lang = "en",
                    expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } } }
                    }
                },
                new MergeTextsTrial {
                    subject = "merge with existing keys",
                    initialTexts = new Dictionary<string, string> { { "hello", "Hi" }, { "foo", "Bar" } },
                    mergeTexts = new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } },
                    lang = "en",
                    expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" }, { "foo", "Bar" }, { "world", "World" } } }
                    }
                },
                new MergeTextsTrial {
                    subject = "merge empty map",
                    initialTexts = new Dictionary<string, string> { { "hello", "Hello" } },
                    mergeTexts = new Dictionary<string, string>(),
                    lang = "en",
                    expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" } } }
                    }
                },
                new MergeTextsTrial {
                    subject = "merge empty map into empty",
                    initialTexts = null,
                    mergeTexts = null,
                    lang = "en",
                    expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string>() }
                    }
                },
                new MergeTextsTrial {
                    subject = "merge different languages",
                    initialTexts = new Dictionary<string, string> { { "hello", "Hello" } },
                    mergeTexts = new Dictionary<string, string> { { "hello", "你好" } },
                    lang = "zh",
                    expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" } } },
                        { "zh", new Dictionary<string, string> { { "hello", "你好" } } }
                    }
                }
            };

            foreach (var tt in dataset)
            {
                var i18n = new I18n("en");
                if (tt.initialTexts != null)
                {
                    i18n.MergeTexts(tt.initialTexts, "en");
                }
                if (tt.mergeTexts != null)
                {
                    i18n.MergeTexts(tt.mergeTexts, tt.lang);
                }
                else
                {
                    i18n.MergeTexts(new Dictionary<string, string>(), tt.lang);
                }

                Assert.Equivalent(tt.expected, i18n.AllTexts());
            }
        }

        class MergeL10nDataTrial
        {
            public string subject { get; set; } = "";
            public string data { get; set; } = "";
            public string lang { get; set; } = "";
            public string expErr { get; set; } = "";
            public Dictionary<string, string>? expected { get; set; }
        }

        [Fact]
        public void TestI18n_MergeL10nData()
        {
            var dataset = new MergeL10nDataTrial[]
            {
                new MergeL10nDataTrial { subject = "valid JSON", data = "{\"hello\":\"Hello\",\"world\":\"World\"}", lang = "en", expErr = "", expected = new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } } },
                new MergeL10nDataTrial { subject = "empty JSON object", data = "{}", lang = "en", expErr = "", expected = new Dictionary<string, string>() },
                new MergeL10nDataTrial { subject = "JSON with unicode", data = "{\"hello\":\"你好\",\"world\":\"世界\"}", lang = "zh", expErr = "", expected = new Dictionary<string, string> { { "hello", "你好" }, { "world", "世界" } } },
                new MergeL10nDataTrial { subject = "invalid JSON", data = "{invalid json", lang = "en", expErr = "Invalid character after parsing property name", expected = null },
                new MergeL10nDataTrial { subject = "malformed JSON", data = "{\"hello\":}", lang = "en", expErr = "Unexpected character encountered while parsing", expected = null },
                new MergeL10nDataTrial { subject = "null JSON", data = "null", lang = "en", expErr = "failed to parse", expected = null },
            };

            foreach (var tt in dataset)
            {
                var i18n = new I18n("en");
                Exception? err = null;
                try
                {
                    i18n.MergeL10nData(Encoding.UTF8.GetBytes(tt.data), tt.lang);
                }
                catch (Exception ex)
                {
                    err = ex;
                }

                if (!string.IsNullOrEmpty(tt.expErr))
                {
                    Assert.NotNull(err);
                    Assert.Contains(tt.expErr, err.Message);
                }
                else
                {
                    Assert.Null(err);
                    Assert.Equivalent(tt.expected, i18n.AllTexts()[tt.lang]);
                }
            }
        }

        [Fact]
        public void TestI18n_MergeL10nFile()
        {
            var langs = new[] { "en", "zh", "und" };
            foreach (var lang in langs)
            {
                var err = Assert.Throws<ArchmageException>(() => new I18n("en").MergeL10nFile("/nonexistent/path/to/l10n.json", lang));
                Assert.Contains("localization file not found", err.Message);
            }
        }

        class GetTextTrial
        {
            public string subject { get; set; } = "";
            public Dictionary<string, Dictionary<string, string>> setupTexts { get; set; } = new();
            public string fallback { get; set; } = "";
            public string key { get; set; } = "";
            public string lang { get; set; } = "";
            public string expErr { get; set; } = "";
            public string expected { get; set; } = "";
        }

        [Fact]
        public void TestI18n_GetText()
        {
            var dataset = new GetTextTrial[]
            {
                new GetTextTrial { subject = "get existing text in requested language", setupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } } }, fallback = "en", key = "hello", lang = "en", expErr = "", expected = "Hello" },
                new GetTextTrial { subject = "fallback to default language", setupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } } }, fallback = "en", key = "hello", lang = "zh", expErr = "", expected = "Hello" },
                new GetTextTrial { subject = "prefer requested language over fallback", setupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } }, { "zh", new Dictionary<string, string> { { "hello", "你好" } } } }, fallback = "en", key = "hello", lang = "zh", expErr = "", expected = "你好" },
                new GetTextTrial { subject = "key not found in any language", setupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } } }, fallback = "en", key = "goodbye", lang = "en", expErr = "not found", expected = "" },
                new GetTextTrial { subject = "key not found in requested or fallback language", setupTexts = new Dictionary<string, Dictionary<string, string>> { { "ja", new Dictionary<string, string> { { "hello", "こんにちは" } } } }, fallback = "en", key = "hello", lang = "zh", expErr = "not found", expected = "" },
                new GetTextTrial { subject = "empty texts", setupTexts = new Dictionary<string, Dictionary<string, string>>(), fallback = "en", key = "hello", lang = "en", expErr = "not found", expected = "" },
                new GetTextTrial { subject = "empty key", setupTexts = new Dictionary<string, Dictionary<string, string>>(), fallback = "en", key = "", lang = "zh", expErr = "not found", expected = "" },
                new GetTextTrial { subject = "key exists in fallback but not requested language", setupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "welcome", "Welcome" } } }, { "es", new Dictionary<string, string> { { "hello", "Hola" } } } }, fallback = "en", key = "welcome", lang = "es", expErr = "", expected = "Welcome" },
            };

            foreach (var tt in dataset)
            {
                var i18n = new I18n(tt.fallback);
                foreach (var kvp in tt.setupTexts)
                {
                    i18n.MergeTexts(kvp.Value, kvp.Key);
                }

                var success = i18n.GetText(tt.key, tt.lang, out var r);
                if (!string.IsNullOrEmpty(tt.expErr))
                {
                    Assert.False(success);
                }
                else
                {
                    Assert.True(success);
                    Assert.Equal(tt.expected, r);
                }
            }
        }

        [Fact]
        public void TestI18n_Text()
        {
            var i18n = new I18n("en");
            i18n.MergeTexts(new Dictionary<string, string> { { "hello", "Hello" } }, "en");
            var r1 = i18n.Text("hello", "en");
            Assert.Equal("Hello", r1);

            Assert.Throws<ArchmageException>(() => i18n.Text("world", "en"));
        }
    }
}
