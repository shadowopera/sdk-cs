using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Conf;
using Shadop.Archmage.Sdk;

#pragma warning disable UNT0006

public class ConfLoader : MonoBehaviour
{
    public enum DemoType
    {
        AddressablesAsync,
        AddressablesConcurrentAsync,
        Resources,
        ResourcesAsync,
        ResourcesConcurrentAsync,
        StreamingAssetsAsync,
        StreamingAssetsConcurrentAsync,
        DirectAccess,
    }

    [Header("Settings")]
    [Space(6)]
    public DemoType _demoType = DemoType.AddressablesConcurrentAsync;

    [Header("Easy Config ID Selection")]
    [Space(6)]
    // In the Inspector, Archmage displays all Config IDs from the configuration table in
    // an intuitive dropdown, making selection easy.
    // See Assets/Editor/ArchmageEditorTools.cs for details.
    public HeroCfgId _heroCfgId = 2;

    public async Task Start()
    {
        Debug.Log($"[ConfLoader] DemoType = {_demoType}");

        switch (_demoType)
        {
            case DemoType.AddressablesAsync:
            case DemoType.AddressablesConcurrentAsync:
                await AddressablesAsyncDemo(_demoType == DemoType.AddressablesConcurrentAsync);
                break;
            case DemoType.Resources:
                ResourcesDemo();
                break;
            case DemoType.ResourcesAsync:
            case DemoType.ResourcesConcurrentAsync:
                await ResourcesAsyncDemo(_demoType == DemoType.ResourcesConcurrentAsync);
                break;
            case DemoType.StreamingAssetsAsync:
            case DemoType.StreamingAssetsConcurrentAsync:
                await StreamingAssetsAsyncDemo(_demoType == DemoType.StreamingAssetsConcurrentAsync);
                break;
            case DemoType.DirectAccess:
                DirectAccessDemo();
                break;
        }
    }

    public static async Task AddressablesAsyncDemo(bool concurrent)
    {
        // 1. Set the root directory for configuration loading.
        // In Addressables, the default Address is usually the project-relative path of the asset.
        // So we use "Assets/Configs" as cfgRoot.
        string cfgRoot = "Assets/Configs";
        string atlasFile = "Assets/Configs/atlas.json";

        // 2. Initialize the Atlas instance.
        var atlas = new ConfigAtlas();

        // 3. Configure loading options.
        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityAddressablesFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        if (concurrent)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new UnityAddressablesFS(), cfgRoot);
            ShowAtlasBasicFeatures(atlas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    public static void ResourcesDemo()
    {
        string cfgRoot = "StaticConfigs";
        string atlasFile = "StaticConfigs/atlas.json";

        var atlas = new ConfigAtlas();

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
            Archmage.LoadAtlas(atlasFile, cfgRoot, atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            InitI18n(new UnityResourcesFS(), cfgRoot);
            ShowAtlasBasicFeatures(atlas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    public static async Task ResourcesAsyncDemo(bool concurrent)
    {
        string cfgRoot = "StaticConfigs";
        string atlasFile = "StaticConfigs/atlas.json";

        var atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityResourcesFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        if (concurrent)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading (Resources)...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new UnityResourcesFS(), cfgRoot);
            ShowAtlasBasicFeatures(atlas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    public static async Task StreamingAssetsAsyncDemo(bool concurrent)
    {
        string cfgRoot = "StreamingConfigs";
        string atlasFile = "StreamingConfigs/atlas.json";

        var atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(new UnityStreamingAssetsFS())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        if (concurrent)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading (StreamingAssets)...");
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, atlas, options);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            await InitI18nAsync(new UnityStreamingAssetsFS(), cfgRoot);
            ShowAtlasBasicFeatures(atlas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    public static void DirectAccessDemo(IProgress<AtlasLoadEvent> progress = null)
    {
        // IMPORTANT: This method works only pre-build. During the build process,
        // Unity packages assets, so regular file access no longer works.
        string cfgRoot = "Assets/Configs";
        string atlasFile = "Assets/Configs/atlas.json";

        var atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithAtlasModifier(atlasJson =>
            {
                atlasJson.Single["prop_floats"]["/"] = atlasJson.Single["prop_floats"]["x5"];
            });

        try
        {
            Debug.Log("[ConfLoader] Starting sync config loading (DirectAccess)...");
            Archmage.LoadAtlas(atlasFile, cfgRoot, atlas, options, progress);
            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");
            InitI18n(new DefaultFS(), cfgRoot);
            ShowAtlasBasicFeatures(atlas);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    public static void InitI18n(IFS fs, string cfgRoot)
    {
        var en = "en";
        var fr = "fr";
        var i18n = new I18n(en);
        i18n.MergeL10nFile($"{cfgRoot}/l10n.json", en, fs);
        i18n.MergeL10nFile($"{cfgRoot}/l10n.fr.json", fr, fs);
        L10n.GetI18n = () => i18n;
        L10n.GetPreferredLanguage = () => "fr";
    }

    public static async Task InitI18nAsync(IFS fs, string cfgRoot)
    {
        var en = "en";
        var fr = "fr";
        var i18n = new I18n(en);
        await i18n.MergeL10nFileAsync($"{cfgRoot}/l10n.json", en, fs);
        await i18n.MergeL10nFileAsync($"{cfgRoot}/l10n.fr.json", fr, fs);
        L10n.GetI18n = () => i18n;
        L10n.GetPreferredLanguage = () => "fr";
    }

    static void ShowAtlasBasicFeatures(ConfigAtlas atlas)
    {
        // IMPORTANT: Set the global ConfigAtlas.
        ConfigAtlas.Instance = atlas;

        // 1. Look up a config entry by ID from a dictionary-based table.
        var cfgId = new HeroCfgId(2);
        atlas.HeroTable.TryLookup(cfgId, out var hero);
        Debug.Log($"[ConfLoader] HeroTable[2]: StartLevel={hero.StartLevel}");

        // 2. Do the same, but in a more convenient way.
        Debug.Log($"[ConfLoader] HeroTable[2]: StartLevel={cfgId.Cfg.StartLevel} (shortcut)");

        // 3. Access a cross-table reference via XRef.Ref.
        var firstChar = atlas.CharacterArray[0];
        Debug.Log($"[ConfLoader] CharacterArray[0]: ID={firstChar.Id}, Attack={firstChar.Attack}");
        Debug.Log($"[ConfLoader] CharacterArray[0].Race.CfgId: {firstChar.Race.CfgId}");
        Debug.Log($"[ConfLoader] CharacterArray[0].Race.Ref.Birthplace: {firstChar.Race.Ref.Birthplace.Text}");

        // 4. Query localized text via L10n.
        Debug.Log($"[ConfLoader] HeroTable[3].HeroName (en, not translated): {atlas.HeroTable[3].HeroName.Text}");
        Debug.Log($"[ConfLoader] GameCfg.XL10n (fr, translated): {atlas.GameCfg.XL10n.Text}");

        // 5. Show Unity Built-in vectors.
        Debug.Log($"[ConfLoader] GameCfg.XVector2 (Vector2Int): {atlas.GameCfg.XVector2}");
        Debug.Log($"[ConfLoader] GameCfg.XVector3 (Vector3): {atlas.GameCfg.XVector3}");
        Debug.Log($"[ConfLoader] GameCfg.XVector4 (Vector4): {atlas.GameCfg.XVector4}");

        // 6. Convert Rgba to Unity Color.
        Debug.Log($"[ConfLoader] GameCfg.XRgba (Rgba): {atlas.GameCfg.XRgba.ToColor()}");
    }
}
