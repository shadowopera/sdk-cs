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
            public string Subject { get; set; } = "";
            public Dictionary<string, string>? InitialTexts { get; set; }
            public Dictionary<string, string>? MergeTexts { get; set; }
            public string Lang { get; set; } = "";
            public Dictionary<string, Dictionary<string, string>> Expected { get; set; } = new();
        }

        [Fact]
        public void TestI18n_MergeTexts()
        {
            var dataset = new MergeTextsTrial[]
            {
                new() {
                    Subject = "merge into empty",
                    InitialTexts = null,
                    MergeTexts = new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } },
                    Lang = "en",
                    Expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } } }
                    }
                },
                new() {
                    Subject = "merge with existing keys",
                    InitialTexts = new Dictionary<string, string> { { "hello", "Hi" }, { "foo", "Bar" } },
                    MergeTexts = new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } },
                    Lang = "en",
                    Expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" }, { "foo", "Bar" }, { "world", "World" } } }
                    }
                },
                new() {
                    Subject = "merge empty map",
                    InitialTexts = new Dictionary<string, string> { { "hello", "Hello" } },
                    MergeTexts = new Dictionary<string, string>(),
                    Lang = "en",
                    Expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" } } }
                    }
                },
                new() {
                    Subject = "merge empty map into empty",
                    InitialTexts = null,
                    MergeTexts = null,
                    Lang = "en",
                    Expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string>() }
                    }
                },
                new() {
                    Subject = "merge different languages",
                    InitialTexts = new Dictionary<string, string> { { "hello", "Hello" } },
                    MergeTexts = new Dictionary<string, string> { { "hello", "你好" } },
                    Lang = "zh",
                    Expected = new Dictionary<string, Dictionary<string, string>> {
                        { "en", new Dictionary<string, string> { { "hello", "Hello" } } },
                        { "zh", new Dictionary<string, string> { { "hello", "你好" } } }
                    }
                }
            };

            foreach (var tt in dataset)
            {
                var i18n = new I18n("en");
                if (tt.InitialTexts != null)
                {
                    i18n.MergeTexts(tt.InitialTexts, "en");
                }
                if (tt.MergeTexts != null)
                {
                    i18n.MergeTexts(tt.MergeTexts, tt.Lang);
                }
                else
                {
                    i18n.MergeTexts(new Dictionary<string, string>(), tt.Lang);
                }

                Assert.Equivalent(tt.Expected, i18n.AllTexts());
            }
        }

        class MergeL10nDataTrial
        {
            public string Subject { get; set; } = "";
            public string Data { get; set; } = "";
            public string Lang { get; set; } = "";
            public string ExpErr { get; set; } = "";
            public Dictionary<string, string>? expected { get; set; }
        }

        [Fact]
        public void TestI18n_MergeL10nData()
        {
            var dataset = new MergeL10nDataTrial[]
            {
                new() { Subject = "valid JSON", Data = "{\"hello\":\"Hello\",\"world\":\"World\"}", Lang = "en", ExpErr = "", expected = new Dictionary<string, string> { { "hello", "Hello" }, { "world", "World" } } },
                new() { Subject = "empty JSON object", Data = "{}", Lang = "en", ExpErr = "", expected = new Dictionary<string, string>() },
                new() { Subject = "JSON with unicode", Data = "{\"hello\":\"你好\",\"world\":\"世界\"}", Lang = "zh", ExpErr = "", expected = new Dictionary<string, string> { { "hello", "你好" }, { "world", "世界" } } },
                new() { Subject = "invalid JSON", Data = "{invalid json", Lang = "en", ExpErr = "Invalid character after parsing property name", expected = null },
                new() { Subject = "malformed JSON", Data = "{\"hello\":}", Lang = "en", ExpErr = "Unexpected character encountered while parsing", expected = null },
                new() { Subject = "null JSON", Data = "null", Lang = "en", ExpErr = "failed to parse", expected = null },
            };

            foreach (var tt in dataset)
            {
                var i18n = new I18n("en");
                Exception? err = null;
                try
                {
                    i18n.MergeL10nData(Encoding.UTF8.GetBytes(tt.Data), tt.Lang);
                }
                catch (Exception ex)
                {
                    err = ex;
                }

                if (!string.IsNullOrEmpty(tt.ExpErr))
                {
                    Assert.NotNull(err);
                    Assert.Contains(tt.ExpErr, err.Message);
                }
                else
                {
                    Assert.Null(err);
                    Assert.Equivalent(tt.expected, i18n.AllTexts()[tt.Lang]);
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
            public string Subject { get; set; } = "";
            public Dictionary<string, Dictionary<string, string>> SetupTexts { get; set; } = new();
            public string Fallback { get; set; } = "";
            public string Key { get; set; } = "";
            public string Lang { get; set; } = "";
            public string ExpErr { get; set; } = "";
            public string Expected { get; set; } = "";
        }

        [Fact]
        public void TestI18n_GetText()
        {
            var dataset = new GetTextTrial[]
            {
                new() { Subject = "get existing text in requested language", SetupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } } }, Fallback = "en", Key = "hello", Lang = "en", ExpErr = "", Expected = "Hello" },
                new() { Subject = "fallback to default language", SetupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } } }, Fallback = "en", Key = "hello", Lang = "zh", ExpErr = "", Expected = "Hello" },
                new() { Subject = "prefer requested language over fallback", SetupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } }, { "zh", new Dictionary<string, string> { { "hello", "你好" } } } }, Fallback = "en", Key = "hello", Lang = "zh", ExpErr = "", Expected = "你好" },
                new() { Subject = "key not found in any language", SetupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "hello", "Hello" } } } }, Fallback = "en", Key = "goodbye", Lang = "en", ExpErr = "not found", Expected = "" },
                new() { Subject = "key not found in requested or fallback language", SetupTexts = new Dictionary<string, Dictionary<string, string>> { { "ja", new Dictionary<string, string> { { "hello", "こんにちは" } } } }, Fallback = "en", Key = "hello", Lang = "zh", ExpErr = "not found", Expected = "" },
                new() { Subject = "empty texts", SetupTexts = new Dictionary<string, Dictionary<string, string>>(), Fallback = "en", Key = "hello", Lang = "en", ExpErr = "not found", Expected = "" },
                new() { Subject = "empty key", SetupTexts = new Dictionary<string, Dictionary<string, string>>(), Fallback = "en", Key = "", Lang = "zh", ExpErr = "not found", Expected = "" },
                new() { Subject = "key exists in fallback but not requested language", SetupTexts = new Dictionary<string, Dictionary<string, string>> { { "en", new Dictionary<string, string> { { "welcome", "Welcome" } } }, { "es", new Dictionary<string, string> { { "hello", "Hola" } } } }, Fallback = "en", Key = "welcome", Lang = "es", ExpErr = "", Expected = "Welcome" },
            };

            foreach (var tt in dataset)
            {
                var i18n = new I18n(tt.Fallback);
                foreach (var kvp in tt.SetupTexts)
                {
                    i18n.MergeTexts(kvp.Value, kvp.Key);
                }

                var success = i18n.GetText(tt.Key, tt.Lang, out var r);
                if (!string.IsNullOrEmpty(tt.ExpErr))
                {
                    Assert.False(success);
                }
                else
                {
                    Assert.True(success);
                    Assert.Equal(tt.Expected, r);
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
