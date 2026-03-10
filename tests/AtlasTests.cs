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
        readonly object _lock = new();
        public List<string> Lines { get; } = new();

        public void Info(string message)
        {
            lock (_lock)
            {
                Lines.Add($"INF {message}");
            }
        }
    }

    class MemoryFS : IFS
    {
        readonly Dictionary<string, byte[]> _fsys;

        public MemoryFS(Dictionary<string, byte[]> fsys)
        {
            _fsys = fsys;
        }

        public bool DirectoryExists(string path)
        {
            return true;
        }

        public bool FileExists(string path)
        {
            var key = path.Replace('\\', '/');
            return _fsys.ContainsKey(key);
        }

        public byte[] ReadAllBytes(string path)
        {
            var key = path.Replace('\\', '/');
            if (_fsys.TryGetValue(key, out var data)) return data;
            throw new FileNotFoundException(path);
        }

        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            var key = path.Replace('\\', '/');
            if (_fsys.TryGetValue(key, out var data)) return Task.FromResult(data);
            throw new FileNotFoundException(path);
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

        static JsonSerializerSettings DefaultJsonSettings()
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

        static AtlasOptions DefaultOpts()
        {
            return new AtlasOptions().WithJsonSettings(DefaultJsonSettings());
        }

        static void CheckUpdateGolden(IAtlas atlas, string goldenDir)
        {
            var updateGolden = Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1";
            if (updateGolden)
            {
                Archmage.DumpAtlas(atlas, goldenDir, DefaultJsonSettings());
            }
            else
            {
                var settings = DefaultJsonSettings();
                foreach (var kvp in atlas.AtlasItems().OrderBy(k => k.Key))
                {
                    if (!kvp.Value.Ready || kvp.Value.Cfg == null) continue;

                    var actualJson = JsonConvert.SerializeObject(kvp.Value.Cfg, settings) + "\n";
                    var goldenFile = Path.Combine(goldenDir, kvp.Key + ".json");

                    Assert.True(File.Exists(goldenFile), $"Golden file {goldenFile} does not exist. Run tests with UPDATE_GOLDEN=1 to generate.");

                    var expectedJson = File.ReadAllText(goldenFile);

                    // Normalize line endings for cross-platform comparison
                    expectedJson = expectedJson.Replace("\r\n", "\n");
                    actualJson = actualJson.Replace("\r\n", "\n");

                    Assert.Equal(expectedJson, actualJson);
                }
            }
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
            var opts = DefaultOpts().WithBlacklist(new[] { "prop_floats" });
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/basic");

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
        }

        [Fact]
        public void TestAtlas_DataVersion()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "prop_floats" });

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

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithAtlasModifier(atlasModifier)
                .WithBlacklist(new[] { "character", "matrix2", "game" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/atlas_modifier");
        }

        [Fact]
        public void TestAtlas_WithWhitelist()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "Item", "game", "weapon-rune" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/whitelist");
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
            CheckUpdateGolden(atlas, "../../../golden/blacklist");
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
                .WithBlacklist(new[] { "prop_floats" })
                .WithOverrideRoot("../../../override/1")
                .WithOverrideRoot("../../../override/2");

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/override_root");
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
        public void TestAtlas_WithOverrideFS()
        {
            var fsys = new Dictionary<string, byte[]>
            {
                { "game.json", Encoding.UTF8.GetBytes("{\"x-string\":\"foo bar\",\"x-map\":{\"7\":\"xxx\",\"9\":\"rab\"}}") },
                { "clutter/magic.json", Encoding.UTF8.GetBytes("{\"200\":{\"name\":\"Power Word: Shield\"}}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "game", "Magic", "weapon-rune" })
                .WithOverrideRoot("../../../override/2")
                .WithOverrideFS(new MemoryFS(fsys));

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/override_fs");
        }

        [Fact]
        public void TestAtlas_WithOverrideRootAndFS()
        {
            var fsys = new Dictionary<string, byte[]>
            {
                { "vtbl/weapon-sword.json", Encoding.UTF8.GetBytes("{\"1000\":{\"name\":\"Dragonfang Blade\",\"price\":1200}}") },
                { "vtbl/weapon-staff.json", Encoding.UTF8.GetBytes("{\"1201\":{\"price\":2050,\"dps\":2}}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "prop_floats" })
                .WithOverrideRoot("../../../override/1")
                .WithOverrideRoot("../../../override/2")
                .WithOverrideFS(new MemoryFS(fsys));

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/override_root_and_fs");
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
                .WithBlacklist(new[] { "prop_floats" })
                .WithCustomLoader(customLoader);

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/custom_loader");
        }

        [Fact]
        public async Task TestAtlas_WithCustomAsyncLoader()
        {
            AtlasItemAsyncLoader customAsyncLoader = async (all, loadAsync, ct) =>
            {
                using var semaphore = new SemaphoreSlim(10);
                var tasks = all.Select(async kvp =>
                {
                    await semaphore.WaitAsync(ct);
                    try
                    {
                        await loadAsync(kvp.Key, kvp.Value, ct);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                await Task.WhenAll(tasks);
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "prop_floats" })
                .WithCustomAsyncLoader(customAsyncLoader);

            var atlas = new ConfigAtlas();
            await Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata", atlas, opts, cancellationToken: TestContext.Current.CancellationToken);
            CheckUpdateGolden(atlas, "../../../golden/custom_loader");
        }

        [Fact]
        public void TestAtlas_LoadAtlas_WithCustomAsyncLoader_Throws()
        {
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithCustomAsyncLoader((all, load, ct) => Task.CompletedTask);
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.Equal("<archmage> Cannot use WithCustomAsyncLoader with synchronous LoadAtlas", err.Message);
        }

        [Fact]
        public async Task TestAtlas_LoadAtlasAsync_WithCustomLoader_Throws()
        {
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithCustomLoader((all, load) => { });
            var err = await Assert.ThrowsAsync<ArchmageException>(async () => await Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata", atlas, opts, cancellationToken: TestContext.Current.CancellationToken));
            Assert.Equal("<archmage> Cannot use WithCustomLoader with asynchronous LoadAtlasAsync", err.Message);
        }

        [Fact]
        public void TestAtlas_NotFoundCallback()
        {
            var atlas = new ConfigAtlas();
            var logger = new ScavengerLogger();
            var opts = DefaultOpts().WithLogger(logger);

            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.Equal("<archmage> cannot find $.single['prop_floats']['/'] in ../../../testdata/atlas.json", err.Message);
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
        public async Task TestAtlas_ContextCancellation()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var atlas = new ConfigAtlas();
            var err = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata", atlas, DefaultOpts(), null, cts.Token));
        }
    }
}
