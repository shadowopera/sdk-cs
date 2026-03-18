using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Conf;
using Shadop.Archmage;

#pragma warning disable UNT0006

public class ConfLoader : MonoBehaviour
{
    public ConfigAtlas Atlas { get; private set; }

    public enum DemoType
    {
        Addressables,
        AddressablesConcurrent,
        Resources,
        ResourcesAsync,
        ResourcesConcurrentAsync,
        StreamingAssetsAsync,
        StreamingAssetsConcurrentAsync,
    }

    [Header("Settings")]
    public DemoType _demoType = DemoType.AddressablesConcurrent;

    async Task Start()
    {
        Debug.Log($"[ConfLoader] DemoType = {_demoType}");

        switch (_demoType)
        {
            case DemoType.Addressables:
            case DemoType.AddressablesConcurrent:
                await AddressablesDemo();
                break;
            case DemoType.Resources:
                ResourcesDemo();
                break;
            case DemoType.ResourcesAsync:
            case DemoType.ResourcesConcurrentAsync:
                await ResourcesAsyncDemo();
                break;
            case DemoType.StreamingAssetsAsync:
            case DemoType.StreamingAssetsConcurrentAsync:
                await StreamingAssetsAsyncDemo();
                break;
        }
    }

    async Task AddressablesDemo()
    {
        // 1. Set the root directory for configuration loading.
        // In Addressables, the default Address is usually the project-relative path of the asset.
        // So we use "Assets/Configs" as cfgRoot.
        string cfgRoot = "Assets/Configs";
        string atlasFile = "Assets/Configs/atlas.json";

        // 2. Initialize the Atlas instance.
        Atlas = new ConfigAtlas();

        // 3. Configure loading options.
        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityAddressablesFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        if (_demoType == DemoType.AddressablesConcurrent)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, Atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new UnityAddressablesFS(), cfgRoot);
            ShowAtlasBasicFeatures();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    void ResourcesDemo()
    {
        string cfgRoot = "StaticConfigs";
        string atlasFile = "StaticConfigs/atlas.json";

        Atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityResourcesFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        try
        {
            Debug.Log("[ConfLoader] Starting sync config loading (Resources)...");
            Archmage.LoadAtlas(atlasFile, cfgRoot, Atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            InitI18n(new UnityResourcesFS(), cfgRoot);
            ShowAtlasBasicFeatures();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    async Task ResourcesAsyncDemo()
    {
        string cfgRoot = "StaticConfigs";
        string atlasFile = "StaticConfigs/atlas.json";

        Atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityResourcesFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        if (_demoType == DemoType.ResourcesConcurrentAsync)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading (Resources)...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, Atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new UnityResourcesFS(), cfgRoot);
            ShowAtlasBasicFeatures();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    async Task StreamingAssetsAsyncDemo()
    {
        string cfgRoot = "StreamingConfigs";
        string atlasFile = "StreamingConfigs/atlas.json";

        Atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityStreamingAssetsFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        if (_demoType == DemoType.StreamingAssetsConcurrentAsync)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading (StreamingAssets)...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, Atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new UnityStreamingAssetsFS(), cfgRoot);
            ShowAtlasBasicFeatures();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    void InitI18n(IFS fs, string cfgRoot)
    {
        var en = "en";
        var fr = "fr";
        var i18n = new I18n(en);
        i18n.MergeL10nFile($"{cfgRoot}/l10n.json", en, fs);
        i18n.MergeL10nFile($"{cfgRoot}/l10n.fr.json", fr, fs);
        L10n.GetI18n = () => i18n;
        L10n.GetPreferredLanguage = () => "fr";
    }

    async Task InitI18nAsync(IFS fs, string cfgRoot)
    {
        var en = "en";
        var fr = "fr";
        var i18n = new I18n(en);
        await i18n.MergeL10nFileAsync($"{cfgRoot}/l10n.json", en, fs);
        await i18n.MergeL10nFileAsync($"{cfgRoot}/l10n.fr.json", fr, fs);
        L10n.GetI18n = () => i18n;
        L10n.GetPreferredLanguage = () => "fr";
    }

    void ShowAtlasBasicFeatures()
    {
        // 1. Look up a config entry by ID from a dictionary-based table.
        Atlas.HeroTable.TryLookup(2, out var hero);
        Debug.Log($"[ConfLoader] HeroTable[2]: StartLevel={hero.StartLevel}");

        // 2. Access a cross-table reference via XRef.Ref.
        var firstChar = Atlas.CharacterArray[0];
        Debug.Log($"[ConfLoader] CharacterArray[0]: ID={firstChar.Id}, Attack={firstChar.Attack}");
        Debug.Log($"[ConfLoader] CharacterArray[0].Race.RawValue: {firstChar.Race.RawValue}");
        Debug.Log($"[ConfLoader] CharacterArray[0].Race.Ref.Birthplace: {firstChar.Race.Ref.Birthplace.Text}");

        // 3. Query localized text via L10n.
        Debug.Log($"[ConfLoader] HeroTable[3].HeroName (en, not translated): {Atlas.HeroTable[3].HeroName.Text}");
        Debug.Log($"[ConfLoader] GameCfg.XL10n (fr, translated): {Atlas.GameCfg.XL10n.Text}");

        // 4. Show Unity Built-in vectors.
        Debug.Log($"[ConfLoader] GameCfg.XVector2 (Vector2Int): {Atlas.GameCfg.XVector2}");
        Debug.Log($"[ConfLoader] GameCfg.XVector3 (Vector3): {Atlas.GameCfg.XVector3}");
        Debug.Log($"[ConfLoader] GameCfg.XVector4 (Vector4): {Atlas.GameCfg.XVector4}");
    }
}
