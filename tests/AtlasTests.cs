using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Newtonsoft.Json;
using Shadop.Archmage;
using Xunit;

namespace Shadop.Archmage.Tests
{
    class ScavengerLogger : IAtlasLogger
    {
        private readonly object _lock = new object();
        public List<string> Lines { get; } = new List<string>();

        public void Info(string message)
        {
            lock (_lock)
            {
                Lines.Add($"INF {message}");
            }
        }

        public void Warn(string message)
        {
            lock (_lock)
            {
                Lines.Add($"WRN {message}");
            }
        }
    }

    class L10nJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(L10n);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return new L10n((string)reader.Value!);
            if (reader.TokenType == JsonToken.Null)
                return new L10n(string.Empty);
            throw new JsonSerializationException($"Expected string for L10n, got {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var l10n = (L10n)value!;
            writer.WriteValue(l10n.ToString());
        }
    }

    public class AtlasTests
    {
        public AtlasTests()
        {
            // Setup working directory so relative paths in tests work
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        }

        private static JsonSerializerSettings DefaultJsonSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
            settings.Converters.Add(new DurationJsonConverter());
            settings.Converters.Add(new RefJsonConverter());
            settings.Converters.Add(new Vec2JsonConverter());
            settings.Converters.Add(new Vec3JsonConverter());
            settings.Converters.Add(new Vec4JsonConverter());
            settings.Converters.Add(new L10nJsonConverter());
            return settings;
        }

        private static AtlasOptions DefaultOpts()
        {
            return new AtlasOptions().WithJsonSettings(DefaultJsonSettings());
        }

        [Fact]
        public void TestAtlas_Basic()
        {
            var en = "en";
            var cn = "zh-CN";
            var i18n = new I18n(en);
            i18n.MergeL10nFile("../../../testdata/l10n.json", en);
            i18n.MergeL10nFile("../../../testdata/l10n.cn.json", cn);
            L10n.GetI18n = () => i18n;

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, DefaultOpts());
            Archmage.DumpAtlas(atlas, "golden/basic", DefaultJsonSettings());

            Assert.Equal("it is a good day", atlas.GameCfg.XL10n.Text(en));
            Assert.Equal("今儿天气真好", atlas.GameCfg.XL10n.Text(cn));

            Assert.True(atlas.ItemTable.TryGetValue(20, out var itemEntry));
            Assert.Equal(20, itemEntry!.Id);

            Assert.NotNull(atlas.CharacterArray[0]!.Race.REF);
            Assert.NotNull(atlas.CharacterArray[1]!.Runes![0].REF);
            Assert.NotNull(atlas.GameCfg.XRef.REF);
            Assert.NotNull(atlas.RaceTable["Dwarf"]!.Referrer2.REF);
            Assert.NotNull(atlas.RefTable[3]!.B.REF);
            Assert.NotNull(atlas.Matrix2Table["key1"]!["key2"]![0][0].REF);
            Assert.Equal(16, atlas.VtItemXTable.Count);
            
            Assert.Null(atlas.DataVersion);
            Assert.NotNull(ConfigAtlas.CodeVersion());
            Assert.Equal("7f3a2b9", ConfigAtlas.CodeVersion().ShortID);

            // Compare golden dump
            Assert.True(Directory.Exists("golden/basic"));
        }

        [Fact]
        public void TestAtlas_DataVersion()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts().WithLogger(logger);

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas_with_version.json", "../../../testdata", atlas, opts);

            Assert.NotNull(atlas.DataVersion);
            Assert.Equal("main", atlas.DataVersion!.Branch);
        }

        [Fact]
        public void TestAtlas_WithAtlasModifier()
        {
            Action<AtlasJson> atlasModifier = (atlasJson) =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
                atlasJson.Unique.Remove("character");
                atlasJson.Unique.Remove("matrix2");
                atlasJson.Single.Remove("game");
            };

            Action<string, AtlasItem> notFound = (key, atlasItem) =>
            {
                if (key != "character" && key != "matrix2" && key != "game")
                {
                    Assert.Fail("unreachable");
                }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithAtlasModifier(atlasModifier)
                .WithNotFoundCallback(notFound);

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            Assert.Contains(logger.Lines, line => line == "WRN <archmage> cannot find $.unique['character'] in ../../../testdata/atlas.json");
            Assert.Contains(logger.Lines, line => line == "WRN <archmage> cannot find $.single['game']['/'] in ../../../testdata/atlas.json");

            int cnt = logger.Lines.Count(line => line.StartsWith("WRN"));
            Assert.Equal(3, cnt);
        }

        [Fact]
        public void TestAtlas_WithWhitelist()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "Item", "game", "prop_floats", "weapon-rune" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            int cnt = logger.Lines.Count(line => line.StartsWith("WRN"));
            Assert.Equal(1, cnt);
        }

        [Fact]
        public void TestAtlas_WithWhitelist_Error()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "Item", "prop_float" });

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> atlas whitelist: unknown item \"prop_float\"", err.Message);
        }

        [Fact]
        public void TestAtlas_WithBlacklist()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "game", "prop_floats", "character" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            int cnt = logger.Lines.Count(line => line.StartsWith("WRN"));
            Assert.Equal(0, cnt);
        }

        [Fact]
        public void TestAtlas_WithBlacklist_Error()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "gm" });

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> atlas blacklist: unknown item \"gm\"", err.Message);
        }

        [Fact]
        public void TestAtlas_WithOverrideRoot()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithOverrideRoot("../../../override/1")
                .WithOverrideRoot("../../../override/2");

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
        }

        [Fact]
        public void TestAtlas_WithOverrideRoot_Error1()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithOverrideRoot("override/9");

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> invalid override root directory \"override/9\"", err.Message);
        }

        [Fact]
        public void TestAtlas_WithCustomLoader()
        {
            AtlasItemLoader customLoader = (all, load) =>
            {
                Parallel.ForEach(all, new ParallelOptions { MaxDegreeOfParallelism = 10 }, kvp =>
                {
                    load(kvp.Key, kvp.Value);
                });
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithCustomLoader(customLoader);

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            Assert.Contains(logger.Lines, line => line == "WRN <archmage> cannot find $.single['prop_floats']['/'] in ../../../testdata/atlas.json");

            int cnt = logger.Lines.Count(line => line.StartsWith("WRN"));
            Assert.Equal(1, cnt);
        }

        [Fact]
        public void TestAtlas_NotFoundCallback()
        {
            var atlas = new ConfigAtlas();
            Action<string, AtlasItem> notFound = (key, atlasItem) =>
            {
                if (key == "prop_floats")
                {
                    atlas.PropFloatsCfg.C = 100;
                    atlasItem.Ready = true;
                }
                else
                {
                    Assert.Fail("unreachable");
                }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithNotFoundCallback(notFound);

            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            int cnt = logger.Lines.Count(line => line.StartsWith("WRN"));
            Assert.Equal(0, cnt);
        }

        [Fact]
        public void TestAtlas_AtlasFileNotFound()
        {
            var atlas = new ConfigAtlas();
            Assert.Throws<DirectoryNotFoundException>(() => Archmage.LoadAtlas("testdata/nonexistent_atlas.json", "testdata", atlas));
        }

        [Fact]
        public void TestAtlas_InvalidAtlasJSON()
        {
            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas_invalid.json", "../../../testdata", atlas));
            Assert.StartsWith("<archmage> invalid \"../../../testdata/atlas_invalid.json\"", err.Message);
        }

        [Fact]
        public void TestAtlas_ConfigFileNotFound()
        {
            Action<AtlasJson> atlasModifier = (atlasJson) =>
            {
                atlasJson.Unique["Item"] = "nonexistent/item.json";
            };
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithAtlasModifier(atlasModifier);
            Assert.Throws<DirectoryNotFoundException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
        }

        [Fact]
        public void TestAtlas_NotFoundCallback_Error()
        {
            Action<string, AtlasItem> notFound = (key, atlasItem) =>
            {
                if (key == "prop_floats")
                {
                    throw new Exception("not found error");
                }
            };
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithNotFoundCallback(notFound);
            var err = Assert.Throws<Exception>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.Equal("not found error", err.Message);
        }

        [Fact]
        public async Task TestAtlas_ContextCancellation()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var atlas = new ConfigAtlas();
            var err = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata", atlas, DefaultOpts(), null, cts.Token));
        }
}
}
