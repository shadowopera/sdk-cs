using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Conf;
using Shadop.Archmage.Sdk;

// Loads the atlas with two override roots through each Unity FS.
// Root 2 lacks some files that root 1 has, so every FS must skip missing override files,
// including UnityResourcesFS whose FileExists always returns true.
public class OverrideTests
{
    class EventCollector : IProgress<AtlasLoadEvent>
    {
        public readonly ConcurrentBag<AtlasLoadEvent> Events = new();
        public void Report(AtlasLoadEvent value) => Events.Add(value);
    }

    [Test]
    public void Resources()
    {
        var atlas = new ConfigAtlas();
        var progress = new EventCollector();
        Archmage.LoadAtlas("StaticConfigs/atlas.json", "StaticConfigs", atlas,
            Options(new UnityResourcesFS(), "StaticConfigOverrides"), progress);
        AssertOverrides(atlas, progress);
    }

    [Test]
    public async Task ResourcesAsync()
    {
        var atlas = new ConfigAtlas();
        var progress = new EventCollector();
        await Archmage.LoadAtlasAsync("StaticConfigs/atlas.json", "StaticConfigs", atlas,
            Options(new UnityResourcesFS(), "StaticConfigOverrides"), progress);
        AssertOverrides(atlas, progress);
    }

    [Test]
    public async Task StreamingAssetsAsync()
    {
        var atlas = new ConfigAtlas();
        var progress = new EventCollector();
        await Archmage.LoadAtlasAsync("StreamingConfigs/atlas.json", "StreamingConfigs", atlas,
            Options(new UnityStreamingAssetsFS(), "StreamingConfigOverrides"), progress);
        AssertOverrides(atlas, progress);
    }

    [Test]
    public async Task AddressablesAsync()
    {
        var atlas = new ConfigAtlas();
        var progress = new EventCollector();
        await Archmage.LoadAtlasAsync("Assets/Configs/atlas.json", "Assets/Configs", atlas,
            Options(new UnityAddressablesFS(), "Assets/ConfigOverrides"), progress);
        AssertOverrides(atlas, progress);
    }

    static AtlasOptions Options(IFS fs, string overrideRoot)
    {
        return new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(fs)
            .WithVariant("balance", "hard")
            .WithOverrideRoot($"{overrideRoot}/1")
            .WithOverrideRoot($"{overrideRoot}/2");
    }

    static void AssertOverrides(ConfigAtlas atlas, EventCollector progress)
    {
        // Overrides applied per key: game and skill come from both roots, hero and item from root 1 only.
        int Applied(string key) => progress.Events.Count(e => e.Key == key && e.Stage == AtlasLoadStage.ApplyingOverride);
        Assert.AreEqual(2, Applied("game"));
        Assert.AreEqual(2, Applied("skill"));
        Assert.AreEqual(1, Applied("hero"));
        Assert.AreEqual(1, Applied("item"));

        // Root 2 wins over root 1.
        Assert.AreEqual(80, atlas.GameCfg.MaxLevel);
        Assert.AreEqual(30, atlas.SkillTable["slash"].Damage.Max);

        // Fields set by a single root.
        Assert.AreEqual(new Vector3(0, 20, -7), atlas.GameCfg.CameraOffset);
        Assert.AreEqual(new Vector4(1, 0.5f, 0.25f, 0.5f), atlas.GameCfg.Tint);
        Assert.AreEqual(130f, atlas.HeroTable[2].Stats!.Atk);
        Assert.AreEqual(new ItemCfgId(104), atlas.HeroTable[3].Weapon.CfgId);
    }
}
