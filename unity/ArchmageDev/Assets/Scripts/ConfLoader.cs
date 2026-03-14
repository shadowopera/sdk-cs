using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Conf;
using Shadop.Archmage;

public class ConfLoader : MonoBehaviour
{
    public ConfigAtlas Atlas { get; private set; }

    async void Start()
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

        var activeScene = SceneManager.GetActiveScene();
        switch (activeScene.name)
        {
            case "ArchmageAddressablesConcurrent":
            {
                options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
                {
                    await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
                });
                break;
            }
        }

        try
        {
            Debug.Log("[ConfLoader] Starting async config loading...");

            // 4. Asynchronously load all configurations.
            await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, Atlas, options);

            Debug.Log("[ConfLoader] ConfigAtlas loaded successfully!");

            // 5. Test reading: print some config data and cross-table references.
            if (Atlas.CharacterArray != null && Atlas.CharacterArray.Count > 0)
            {
                var firstChar = Atlas.CharacterArray[0];
                if (firstChar != null)
                {
                    Debug.Log($"[ConfLoader] Loaded first character: ID={firstChar.Id}, Attack={firstChar.Attack}");

                    // Test if cross-table reference (XRef) resolution was successful.
                    if (firstChar.Race.Ref != null)
                    {
                        Debug.Log($"[ConfLoader] Character race info resolved: {firstChar.Race.Ref.Id}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfLoader] Failed to load config: {ex}");
        }
    }
}
