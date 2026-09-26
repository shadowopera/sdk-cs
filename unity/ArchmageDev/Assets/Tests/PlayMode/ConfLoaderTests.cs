using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Conf;
using Conf.Enums;
using Shadop.Archmage.Sdk;

// Runs every ConfLoader demo in Play Mode and checks the features ShowAtlasBasicFeatures showcases.
// A demo that fails logs an error instead of throwing, which the Test Framework reports as a failure.
public class ConfLoaderTests
{
    [SetUp]
    public void SetUp()
    {
        ConfigAtlas.Instance = null;
        L10n.GetI18n = null;
        L10n.GetPreferredLanguage = null;
    }

    [Test]
    public async Task AddressablesAsync()
    {
        await ConfLoader.AddressablesAsyncDemo(false, CancellationToken.None);
        AssertAtlas();
    }

    [Test]
    public async Task AddressablesConcurrentAsync()
    {
        await ConfLoader.AddressablesAsyncDemo(true, CancellationToken.None);
        AssertAtlas();
    }

    [Test]
    public void Resources()
    {
        ConfLoader.ResourcesDemo();
        AssertAtlas();
    }

    [Test]
    public async Task ResourcesAsync()
    {
        await ConfLoader.ResourcesAsyncDemo(false, CancellationToken.None);
        AssertAtlas();
    }

    [Test]
    public async Task ResourcesConcurrentAsync()
    {
        await ConfLoader.ResourcesAsyncDemo(true, CancellationToken.None);
        AssertAtlas();
    }

    [Test]
    public async Task StreamingAssetsAsync()
    {
        await ConfLoader.StreamingAssetsAsyncDemo(false, CancellationToken.None);
        AssertAtlas();
    }

    [Test]
    public async Task StreamingAssetsConcurrentAsync()
    {
        await ConfLoader.StreamingAssetsAsyncDemo(true, CancellationToken.None);
        AssertAtlas();
    }

    [Test]
    public void DirectAccess()
    {
        ConfLoader.DirectAccessDemo();
        AssertAtlas();
    }

    static void AssertAtlas()
    {
        var atlas = ConfigAtlas.Instance;
        Assert.IsNotNull(atlas, "ConfigAtlas.Instance was not set");
        Assert.IsNotNull(L10n.GetI18n, "L10n.GetI18n was not set");

        // Lookup by ID.
        var hero = atlas.HeroTable[2];
        Assert.AreEqual(255, hero.Level);
        Assert.AreEqual(255, new HeroCfgId(2).Cfg.Level);

        // Cross-table references.
        Assert.AreEqual(new ItemCfgId(102), hero.Weapon.CfgId);
        Assert.AreEqual("Oak Staff", hero.Weapon.Ref.Name);
        Assert.AreEqual("Silverwood", hero.Race.Ref.Birthplace.Text);

        // Localized text: fr when translated, en otherwise.
        Assert.AreEqual("Arthur Pendragon", atlas.HeroTable[1].Name.Text);
        Assert.AreEqual("Légendes d’Avalon", atlas.GameCfg.Title.Text);

        // Enums, bitflags and localized enum items.
        Assert.AreEqual(HeroClass.Mage, hero.Class);
        Assert.AreEqual(Element.Water | Element.Earth, hero.Elements);
        Assert.AreEqual("Guerrier", new L10n(HeroClass.Warrior.GetL10nKey()).Text);

        // Unity built-in vectors.
        Assert.AreEqual(new Vector3Int(10, 0, -5), atlas.HeroTable[1].SpawnPos);
        Assert.AreEqual(new Vector2(0, 1), atlas.HeroTable[1].Facing);
        Assert.AreEqual(new Vector2Int(64, 48), atlas.GameCfg.GridSize);
        Assert.AreEqual(new Vector3(0, 10.5f, -7), atlas.GameCfg.CameraOffset);
        Assert.AreEqual(new Vector4(1, 0.5f, 0.25f, 1), atlas.GameCfg.Tint);

        // Rgba to Unity Color.
        Assert.AreEqual(new Color(0x10 / 255f, 0x20 / 255f, 0x30 / 255f, 1f), atlas.GameCfg.BgColor.ToColor());

        // Variant item: the atlas modifier makes "hard" the default.
        Assert.AreEqual(1.5f, atlas.BalanceCfg.HpScale);
    }
}
