using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Newtonsoft.Json;
using Shadop.Archmage.Sdk;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public partial class AtlasTests
    {
        public AtlasTests()
        {
            // Setup working directory so relative paths in tests work
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
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
            L10n.GetPreferredLanguage = () => cn;

            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithBlacklist(new[] { "prop_floats" });
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/basic");

            var ok = atlas.GameCfg.XL10n.GetText(en, out var text);
            Assert.Equal("it is a good day", text);
            Assert.Equal("今儿天气真好", atlas.GameCfg.XL10n.Text);

            Assert.True(atlas.ItemTable.TryGetValue(20, out var itemEntry));
            Assert.Equal(20, itemEntry!.Id);

            Assert.NotNull(atlas.CharacterArray[0]!.Race.Ref);
            Assert.NotNull(atlas.CharacterArray[1]!.Runes![0].Ref);
            Assert.NotNull(atlas.GameCfg.XRef.Ref);
            Assert.NotNull(atlas.RaceTable["Dwarf"]!.Referrer2.Ref);
            Assert.NotNull(atlas.RefTable[3]!.B.Ref);
            Assert.NotNull(atlas.Matrix2Table["key1"]!["key2"]![0][0].Ref);
            Assert.Equal(16, atlas.VtItemXTable.Count);

            Assert.Null(atlas.DataVersion);
            Assert.NotNull(ConfigAtlas.CodeVersion);
            Assert.Equal("7f3a2b9", ConfigAtlas.CodeVersion.ShortID);
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
            Assert.StartsWith("<archmage> Atlas whitelist: unknown item \"prop_float\"", err.Message);
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
            Assert.StartsWith("<archmage> Atlas blacklist: unknown item \"gm\".", err.Message);
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
            Assert.StartsWith("<archmage> Invalid override root directory \"override/9\"", err.Message);
        }


        [Fact]
        public void TestAtlas_WithFS()
        {
            var fsys = new Dictionary<string, byte[]>
            {
                { "testdata/atlas.json", System.Text.Encoding.UTF8.GetBytes("{\"version\":{\"branch\":\"test-branch\",\"id\":\"123456\"},\"single\":{\"game\":{\"/\":\"game.json\"}},\"multiple\":{},\"unique\":{}}") },
                { "testdata/game.json", System.Text.Encoding.UTF8.GetBytes("{\"x-string\":\"hello memory fs\"}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithFS(new MemoryFS(fsys))
                .WithWhitelist(new[] { "game" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("testdata/atlas.json", "testdata", atlas, opts);

            Assert.Equal("hello memory fs", atlas.GameCfg.XString);
            Assert.Equal("test-branch", atlas.DataVersion!.Branch);
            Assert.Equal("123456", atlas.DataVersion!.ID);
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
            var events = new System.Collections.Concurrent.ConcurrentBag<AtlasLoadEvent>();
            var progress = new SyncProgress<AtlasLoadEvent>(events.Add);

            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts, progress);
            CheckUpdateGolden(atlas, "../../../golden/override_root_and_fs");

            // Verify events for a specific key to avoid brittle global counts
            var vtItemXEvents = events.Where(e => e.Key == "vtItemX").ToList();
            Assert.NotEmpty(vtItemXEvents);
            Assert.Equal(1, vtItemXEvents.Count(e => e.Stage == AtlasLoadStage.StartProcessing));
            Assert.Equal(5, vtItemXEvents.Count(e => e.Stage == AtlasLoadStage.StartReading));
            Assert.Equal(5, vtItemXEvents.Count(e => e.Stage == AtlasLoadStage.StartParsing));
            Assert.Equal(3, vtItemXEvents.Count(e => e.Stage == AtlasLoadStage.StartReadingOverride));
            Assert.Equal(3, vtItemXEvents.Count(e => e.Stage == AtlasLoadStage.ApplyingOverride));
            Assert.Equal(1, vtItemXEvents.Count(e => e.Stage == AtlasLoadStage.Completed));
        }

        [Fact]
        public void TestAtlas_WithLoadStrategy()
        {
            AtlasLoadStrategy loadStrategy = (all, load) =>
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
                .WithLoadStrategy(loadStrategy);

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/custom_loader");
        }

        [Fact]
        public async Task TestAtlas_WithAsyncLoadStrategy()
        {
            AtlasAsyncLoadStrategy asyncLoadStrategy = async (all, loadAsync, ct) =>
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
                .WithAsyncLoadStrategy(asyncLoadStrategy);

            var atlas = new ConfigAtlas();
            var events = new System.Collections.Concurrent.ConcurrentBag<AtlasLoadEvent>();
            var progress = new SyncProgress<AtlasLoadEvent>(events.Add);

            await Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata", atlas, opts, progress, cancellationToken: TestContext.Current.CancellationToken);
            CheckUpdateGolden(atlas, "../../../golden/custom_async_loader");

            // Verify events for a specific key to avoid brittle global counts
            var vtSkillEvents = events.Where(e => e.Key == "vtSkill").ToList();
            Assert.NotEmpty(vtSkillEvents);
            Assert.Equal(1, vtSkillEvents.Count(e => e.Stage == AtlasLoadStage.StartProcessing));
            Assert.Equal(2, vtSkillEvents.Count(e => e.Stage == AtlasLoadStage.StartReading));
            Assert.Equal(2, vtSkillEvents.Count(e => e.Stage == AtlasLoadStage.StartParsing));
            Assert.Equal(0, vtSkillEvents.Count(e => e.Stage == AtlasLoadStage.StartReadingOverride));
            Assert.Equal(0, vtSkillEvents.Count(e => e.Stage == AtlasLoadStage.ApplyingOverride));
            Assert.Equal(1, vtSkillEvents.Count(e => e.Stage == AtlasLoadStage.Completed));
        }

        [Fact]
        public void TestAtlas_LoadAtlas_WithAsyncLoadStrategy_Throws()
        {
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithAsyncLoadStrategy((all, load, ct) => Task.CompletedTask);
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.Equal("<archmage> Cannot use WithAsyncLoadStrategy with synchronous LoadAtlas.", err.Message);
        }

        [Fact]
        public async Task TestAtlas_LoadAtlasAsync_WithLoadStrategy_Throws()
        {
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithLoadStrategy((all, load) => { });
            var err = await Assert.ThrowsAsync<ArchmageException>(async () => await Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata", atlas, opts, cancellationToken: TestContext.Current.CancellationToken));
            Assert.Equal("<archmage> Cannot use WithLoadStrategy with asynchronous LoadAtlasAsync.", err.Message);
        }

        [Fact]
        public void TestAtlas_NotFoundCallback()
        {
            var atlas = new ConfigAtlas();
            var logger = new ScavengerLogger();
            var opts = DefaultOpts().WithLogger(logger);

            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"prop_floats\"", err.Message);
            Assert.NotNull(err.InnerException);
            Assert.IsType<Exception>(err.InnerException);
            Assert.Equal("Could not find $.single['prop_floats']['/'] in ../../../testdata/atlas.json.", err.InnerException.Message);
        }

        [Fact]
        public void TestAtlas_AtlasFileNotFound()
        {
            var atlas = new ConfigAtlas();
            Assert.Throws<FileNotFoundException>(() => Archmage.LoadAtlas("../../../testdata/nonexistent_atlas.json", "../../../testdata", atlas));
        }

        [Fact]
        public void TestAtlas_InvalidAtlasJSON()
        {
            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas_invalid.json", "../../../testdata", atlas));
            Assert.StartsWith("<archmage> Invalid \"../../../testdata/atlas_invalid.json\"", err.Message);
        }

        [Fact]
        public void TestAtlas_ConfigFileNotFound()
        {
            Action<AtlasJson> atlasModifier = (atlasJson) =>
            {
                atlasJson.Unique["Item"] = "clutter/nonexistent_item.json";
            };
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts().WithAtlasModifier(atlasModifier);
            var err = Assert.Throws<ArchmageException>(() => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"Item\"", err.Message);
            Assert.IsType<FileNotFoundException>(err.InnerException);
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
