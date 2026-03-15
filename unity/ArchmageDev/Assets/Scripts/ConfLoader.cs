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
            LogFirstCharacter();
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
            LogFirstCharacter();
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
            LogFirstCharacter();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    async Task StreamingAssetsAsyncDemo()
    {
        string cfgRoot = "WebConfigs";
        string atlasFile = "WebConfigs/atlas.json";

        Atlas = new ConfigAtlas();

        var options = new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
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
            LogFirstCharacter();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }

    void LogFirstCharacter()
    {
        if (Atlas.CharacterArray == null || Atlas.CharacterArray.Count == 0)
            return;

        var firstChar = Atlas.CharacterArray[0];
        if (firstChar == null)
            return;

        Debug.Log($"[ConfLoader] Loaded first character: ID={firstChar.Id}, Attack={firstChar.Attack}");

        if (firstChar.Race.Ref != null)
            Debug.Log($"[ConfLoader] Character race info resolved: {firstChar.Race.Ref.Id}");
    }
}
