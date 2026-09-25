using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Conf.Enums;
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
            var opts = DefaultOpts().WithBlacklist(new[] { "balance" });
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/basic");

            Assert.True(CheckXRefs(atlas) > 0);

            Assert.True(atlas.GameCfg.Title.GetText(en, out var text));
            Assert.Equal("Legends of Avalon", text);
            Assert.Equal("阿瓦隆传说", atlas.GameCfg.Title.Text);
            Assert.Equal("Arthur Pendragon", atlas.HeroTable[1].Name.Text);
            Assert.Equal("Silverwood", atlas.RaceTable["Elf"].Birthplace.Text);
            Assert.True(atlas.HeroTable[4].Name.GetText(cn, out var blank));
            Assert.Equal("", blank);
            Assert.Equal("", atlas.HeroTable[4].Name.Text);

            Assert.Equal("enum::HeroClass.Warrior", HeroClass.Warrior.GetL10nKey());
            Assert.Equal("", HeroClass.Ranger.GetL10nKey());
            Assert.Equal("战士", i18n.Text(HeroClass.Warrior.GetL10nKey(), cn));

            Assert.True(atlas.ItemTable.TryGetValue(101, out var itemEntry));
            Assert.Equal(101, itemEntry.Id);

            ConfigAtlas.Instance = atlas;
            Assert.Same(itemEntry, new ItemCfgId(101).Cfg);
            Assert.Same(atlas.RaceTable["Elf"], new RaceCfgId("Elf").Cfg);

            Assert.NotNull(atlas.DataVersion);
            Assert.Equal("v1.0.0", atlas.DataVersion!.Semver);
            Assert.Null(atlas.DataVersion.Branch);
            Assert.NotNull(ConfigAtlas.CodeVersion);
            Assert.Equal("7f3a2b9", ConfigAtlas.CodeVersion.ShortID);
        }

        [Fact]
        public void TestAtlas_DataVersion()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance" });

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
                atlasJson.Variant["balance"]["/"] = atlasJson.Variant["balance"]["hard"];
                atlasJson.Unique.Remove("chapter");
                atlasJson.Unique.Remove("route");
                atlasJson.Variant.Remove("game");
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithAtlasModifier(atlasModifier)
                .WithBlacklist(new[] { "chapter", "route", "game" });

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
                .WithWhitelist(new[] { "hero", "item", "Race", "skill" });

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
                .WithWhitelist(new[] { "item", "balanc" });

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Atlas whitelist: unknown item \"balanc\"", err.Message);
        }

        [Fact]
        public void TestAtlas_WithBlacklist()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance", "game", "chapter" });

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
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Atlas blacklist: unknown item \"gm\".", err.Message);
        }

        [Theory]
        [InlineData("easy")]
        [InlineData("hard")]
        public void TestAtlas_WithVariant(string variant)
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "balance" })
                .WithVariant("balance", variant);

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/variant_" + variant);

            Assert.Equal(variant, atlas.AtlasItems()["balance"].Variant);
        }

        [Fact]
        public void TestAtlas_WithVariant_Default()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "game", "hero", "item", "Race", "skill" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            var items = atlas.AtlasItems();
            Assert.Equal("/", items["game"].Variant);
            Assert.Equal("", items["hero"].Variant);
            Assert.Equal("", items["skill"].Variant);
        }

        [Fact]
        public void TestAtlas_WithVariant_LastWins()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "balance" })
                .WithVariant("balance", "easy")
                .WithVariant("balance", "hard");

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);
            CheckUpdateGolden(atlas, "../../../golden/variant_hard");
        }

        [Fact]
        public void TestAtlas_WithVariant_UnknownItem()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance" })
                .WithVariant("balanc", "hard");

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Atlas variant: unknown item \"balanc\".", err.Message);
        }

        [Fact]
        public void TestAtlas_WithVariant_EmptyVariant()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "balance" })
                .WithVariant("balance", "");

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Atlas variant: empty variant for item \"balance\".", err.Message);
        }

        [Fact]
        public void TestAtlas_WithVariant_NullArgument()
        {
            var opts = DefaultOpts();
            Assert.Throws<ArgumentNullException>(() => opts.WithVariant(null!, "hard"));
            Assert.Throws<ArgumentNullException>(() => opts.WithVariant("balance", null!));
        }

        [Fact]
        public void TestAtlas_WithVariant_NotFound()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "balance" })
                .WithVariant("balance", "medium");

            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"balance\"", err.Message);
            Assert.NotNull(err.InnerException);
            Assert.Equal("Could not find $.variant['balance']['medium'] in ../../../testdata/atlas.json.",
                err.InnerException.Message);
        }

        [Fact]
        public void TestAtlas_WithVariant_SkippedItem()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance" })
                .WithVariant("balance", "medium")
                .WithVariant("hero", "hard");

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts);

            var items = atlas.AtlasItems();
            Assert.False(items["balance"].Ready);
            Assert.Equal("", items["hero"].Variant);
        }

        [Fact]
        public void TestAtlas_WithOverrideRoot()
        {
            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance" })
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
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Invalid override root directory \"override/9\"", err.Message);
        }


        [Fact]
        public void TestAtlas_WithFS()
        {
            var fsys = new Dictionary<string, byte[]>
            {
                { "testdata/atlas.json", Encoding.UTF8.GetBytes(
                    "{\"version\":{\"branch\":\"test-branch\",\"id\":\"123456\"},\"variant\":{\"game\":{\"/\":\"game.json\"}},\"many\":{},\"unique\":{}}") },
                { "testdata/game.json", Encoding.UTF8.GetBytes("{\"bgm\":\"hello memory fs\"}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithFS(new MemoryFS(fsys))
                .WithWhitelist(new[] { "game" });

            var atlas = new ConfigAtlas();
            Archmage.LoadAtlas("testdata/atlas.json", "testdata", atlas, opts);

            Assert.Equal("hello memory fs", atlas.GameCfg.Bgm);
            Assert.Equal("test-branch", atlas.DataVersion!.Branch);
            Assert.Equal("123456", atlas.DataVersion!.ID);
        }

        [Fact]
        public void TestAtlas_WithOverrideFS()
        {
            var fsys = new Dictionary<string, byte[]>
            {
                { "game.json", Encoding.UTF8.GetBytes("{\"bgm\":\"audio/night.ogg\",\"levelRewards\":{\"40\":104},\"motd\":{\"item0\":\"Hello\"}}") },
                { "item.json", Encoding.UTF8.GetBytes("{\"105\":{\"name\":\"Aegis of Camelot\",\"tags\":[\"shield\"]}}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithWhitelist(new[] { "game", "hero", "item", "Race", "skill" })
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
                { "vtbl/skill-magic.json", Encoding.UTF8.GetBytes("{\"heal\":{\"mana\":25,\"cooldown\":[0,8]}}") },
                { "vtbl/skill-passive.json", Encoding.UTF8.GetBytes("{\"aura\":{\"radius\":7}}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance" })
                .WithOverrideRoot("../../../override/1")
                .WithOverrideRoot("../../../override/2")
                .WithOverrideFS(new MemoryFS(fsys));

            var atlas = new ConfigAtlas();
            var events = new System.Collections.Concurrent.ConcurrentBag<AtlasLoadEvent>();
            var progress = new SyncProgress<AtlasLoadEvent>(events.Add);

            Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts, progress);
            CheckUpdateGolden(atlas, "../../../golden/override_root_and_fs");

            // Verify events for a specific key to avoid brittle global counts
            var skillEvents = events.Where(e => e.Key == "skill").ToList();
            Assert.NotEmpty(skillEvents);
            Assert.Equal(1, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartProcessing));
            Assert.Equal(3, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartReading));
            Assert.Equal(3, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartParsing));
            Assert.Equal(4, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartReadingOverride));
            Assert.Equal(4, skillEvents.Count(e => e.Stage == AtlasLoadStage.ApplyingOverride));
            Assert.Equal(1, skillEvents.Count(e => e.Stage == AtlasLoadStage.Completed));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestAtlas_WithOverrideFS_FileExistsFalsePositive(bool isAsync)
        {
            var fsys = new Dictionary<string, byte[]>
            {
                { "vtbl/skill-magic.json", Encoding.UTF8.GetBytes("{\"heal\":{\"mana\":25,\"cooldown\":[0,8]}}") },
                { "vtbl/skill-passive.json", Encoding.UTF8.GetBytes("{\"aura\":{\"radius\":7}}") }
            };

            var logger = new ScavengerLogger();
            var opts = DefaultOpts()
                .WithLogger(logger)
                .WithBlacklist(new[] { "balance" })
                .WithOverrideRoot("../../../override/1")
                .WithOverrideRoot("../../../override/2")
                .WithOverrideFS(new MemoryFS(fsys, alwaysExists: true));

            var atlas = new ConfigAtlas();
            var events = new System.Collections.Concurrent.ConcurrentBag<AtlasLoadEvent>();
            var progress = new SyncProgress<AtlasLoadEvent>(events.Add);

            if (isAsync)
                await Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata",
                    atlas, opts, progress, cancellationToken: TestContext.Current.CancellationToken);
            else
                Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts, progress);
            CheckUpdateGolden(atlas, "../../../golden/override_root_and_fs");

            // Missing override files are attempted but skipped.
            var skillEvents = events.Where(e => e.Key == "skill").ToList();
            Assert.Equal(4, skillEvents.Count(e => e.Stage == AtlasLoadStage.ApplyingOverride));
            Assert.True(skillEvents.Count(e => e.Stage == AtlasLoadStage.StartReadingOverride) > 4);
        }

        [Fact]
        public void TestAtlas_NotFoundCallback()
        {
            var atlas = new ConfigAtlas();
            var logger = new ScavengerLogger();
            var opts = DefaultOpts().WithLogger(logger);

            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"balance\"", err.Message);
            Assert.NotNull(err.InnerException);
            Assert.IsType<Exception>(err.InnerException);
            Assert.Equal("Could not find $.variant['balance']['/'] in ../../../testdata/atlas.json.",
                err.InnerException.Message);
        }

        [Fact]
        public void TestAtlas_AtlasFileNotFound()
        {
            var atlas = new ConfigAtlas();
            Assert.Throws<FileNotFoundException>(
                () => Archmage.LoadAtlas("../../../testdata/nonexistent_atlas.json", "../../../testdata", atlas));
        }

        [Fact]
        public void TestAtlas_InvalidAtlasJSON()
        {
            var atlas = new ConfigAtlas();
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas_invalid.json", "../../../testdata", atlas));
            Assert.StartsWith("<archmage> Invalid \"../../../testdata/atlas_invalid.json\"", err.Message);
        }

        [Fact]
        public void TestAtlas_ConfigFileNotFound()
        {
            Action<AtlasJson> atlasModifier = (atlasJson) =>
            {
                atlasJson.Unique["item"] = "nonexistent_item.json";
            };
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts()
                .WithAtlasModifier(atlasModifier)
                .WithBlacklist(new[] { "balance" });
            var err = Assert.Throws<ArchmageException>(
                () => Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"item\"", err.Message);
            Assert.IsType<FileNotFoundException>(err.InnerException);
        }

        [Fact]
        public async Task TestAtlas_ContextCancellation()
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            var atlas = new ConfigAtlas();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => Archmage.LoadAtlasAsync(
                    "../../../testdata/atlas.json", "../../../testdata", atlas, DefaultOpts(), null, cts.Token));
        }
    }
}
