using System;
using System.Linq;
using System.Threading.Tasks;
using Conf;
using Conf.Enums;
using Godot;
using Environment = System.Environment;
using Shadop.Archmage.Sdk;

namespace ArchmageDev.Tests
{
    // Loads the atlas from res://configs and checks the loaded content and the threads the loader calls back on.
    static class LoadTests
    {
        public const string CfgRoot = "res://configs";
        public const string AtlasFile = "res://configs/atlas.json";

        [GodotTest]
        public static Task LoadAtlas()
        {
            var atlas = new ThreadProbeAtlas();
            Archmage.LoadAtlas(AtlasFile, CfgRoot, atlas, Options(new ThreadProbeLog()));

            AssertAtlas(atlas.Inner);
            AssertMainThread(atlas.BindRefsThreadId, "BindRefs");
            AssertMainThread(atlas.OnLoadedThreadId, "OnLoaded");
            return Task.CompletedTask;
        }

        [GodotTest]
        public static async Task LoadAtlasAsync()
        {
            var atlas = new ThreadProbeAtlas();
            await Archmage.LoadAtlasAsync(AtlasFile, CfgRoot, atlas, Options(new ThreadProbeLog()));
            AssertMainThread(Environment.CurrentManagedThreadId, "The code after await");

            AssertAtlas(atlas.Inner);
            AssertMainThread(atlas.BindRefsThreadId, "BindRefs");
            AssertMainThread(atlas.OnLoadedThreadId, "OnLoaded");
        }

        [GodotTest]
        public static async Task LoadAtlasAsync_MaxConcurrency1()
        {
            var atlas = new ThreadProbeAtlas();
            await Archmage.LoadAtlasAsync(AtlasFile, CfgRoot, atlas,
                Options(new ThreadProbeLog()).WithMaxConcurrency(1));
            AssertMainThread(Environment.CurrentManagedThreadId, "The code after await");

            AssertAtlas(atlas.Inner);
            AssertMainThread(atlas.BindRefsThreadId, "BindRefs");
            AssertMainThread(atlas.OnLoadedThreadId, "OnLoaded");
        }

        [GodotTest]
        public static async Task LoadAtlasAsync_WorkerThreadLoading()
        {
            var atlas = new ThreadProbeAtlas();
            await Archmage.LoadAtlasAsync(AtlasFile, CfgRoot, atlas, Options(new ThreadProbeLog()),
                workerThreadLoading: true);
            AssertMainThread(Environment.CurrentManagedThreadId, "The code after await");

            AssertAtlas(atlas.Inner);
            Assert.AreNotEqual(TestRunner.MainThreadId, atlas.BindRefsThreadId, "BindRefs ran on the main thread.");
            AssertMainThread(atlas.OnLoadedThreadId, "OnLoaded");
        }

        [GodotTest]
        public static async Task LoadAtlasAsync_MainThreadParsing()
        {
            var atlas = new ThreadProbeAtlas();
            var log = new ThreadProbeLog();
            await Archmage.LoadAtlasAsync(AtlasFile, CfgRoot, atlas, Options(log).WithMainThreadParsing(),
                progress: log);
            AssertMainThread(Environment.CurrentManagedThreadId, "The code after await");

            AssertAtlas(atlas.Inner);
            AssertMainThread(atlas.BindRefsThreadId, "BindRefs");
            Assert.IsTrue(log.LoggerThreadIds.Count > 0, "The logger was not called.");
            Assert.IsTrue(log.LoggerThreadIds.All(id => id == TestRunner.MainThreadId),
                "The logger was called on another thread.");
            Assert.IsTrue(log.ProgressThreadIds.Count > 0, "Progress was not reported.");
            Assert.IsTrue(log.ProgressThreadIds.All(id => id == TestRunner.MainThreadId),
                "Progress was reported on another thread.");
        }

        public static AtlasOptions Options(ThreadProbeLog log)
        {
            return new AtlasOptions()
                .WithLogger(log)
                .WithJsonSettings(GodotJsonSettingsFactory.Create())
                .WithFS(new GodotFileAccessFS())
                .WithVariant("balance", "hard");
        }

        public static void AssertMainThread(int threadId, string what)
        {
            Assert.AreEqual(TestRunner.MainThreadId, threadId, $"{what} did not run on the main thread.");
        }

        public static void InitI18n()
        {
            var i18n = new I18n("en");
            i18n.MergeL10nFile($"{CfgRoot}/l10n.json", "en", new GodotFileAccessFS());
            i18n.MergeL10nFile($"{CfgRoot}/l10n.fr.json", "fr", new GodotFileAccessFS());
            L10n.GetI18n = () => i18n;
            L10n.GetPreferredLanguage = () => "fr";
        }

        // The same checks as AssertAtlas in the Unity ConfLoaderTests, with Godot vectors and colors.
        public static void AssertAtlas(ConfigAtlas atlas)
        {
            ConfigAtlas.Instance = atlas;
            InitI18n();

            // Lookup by ID.
            var hero = atlas.HeroTable[2];
            Assert.AreEqual(255, hero.Level);
            Assert.AreEqual(255, new HeroCfgId(2).Cfg.Level);

            // Cross-table references.
            Assert.AreEqual(new ItemCfgId(102), hero.Weapon.CfgId);
            Assert.AreEqual("Oak Staff", hero.Weapon.Ref!.Name);
            Assert.AreEqual("Silverwood", hero.Race.Ref!.Birthplace.Text);

            // Localized text: fr when translated, en otherwise.
            Assert.AreEqual("Arthur Pendragon", atlas.HeroTable[1].Name.Text);
            Assert.AreEqual("Légendes d’Avalon", atlas.GameCfg.Title.Text);

            // Enums, bitflags and localized enum items.
            Assert.AreEqual(HeroClass.Mage, hero.Class);
            Assert.AreEqual(Element.Water | Element.Earth, hero.Elements);
            Assert.AreEqual("Guerrier", new L10n(HeroClass.Warrior.GetL10nKey()).Text);

            // Godot built-in vectors.
            Assert.AreEqual(new Vector3I(10, 0, -5), atlas.HeroTable[1].SpawnPos);
            Assert.AreEqual(new Vector2(0, 1), atlas.HeroTable[1].Facing);
            Assert.AreEqual(new Vector2I(64, 48), atlas.GameCfg.GridSize);
            Assert.AreEqual(new Vector3(0, 10.5f, -7), atlas.GameCfg.CameraOffset);
            Assert.AreEqual(new Vector4(1, 0.5f, 0.25f, 1), atlas.GameCfg.Tint);

            // Rgba to Godot Color.
            Assert.AreEqual(new Color(0x10 / 255f, 0x20 / 255f, 0x30 / 255f, 1f), atlas.GameCfg.BgColor.ToColor());

            // Variant item: WithVariant selects "hard".
            Assert.AreEqual(1.5f, atlas.BalanceCfg.HpScale);
        }
    }
}
