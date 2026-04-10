using UnityEditor;
using UnityEngine;
using System;
using Shadop.Archmage.Sdk;
using System.Threading;

namespace Conf.Editor
{
    public static class ArchmageEditorTools
    {
        [MenuItem("Tools/Reload Game Configs for Editor")]
        public static void ReloadGameConfigs()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("<archmage> Do not reload game configs for Unity Editor in Play mode.");
                return;
            }

            var total = 1;
            var completed = 0;
            var slowMotion = false;
            var progress = new Progress<AtlasLoadEvent>(evt =>
            {
                switch (evt.Stage)
                {
                    case AtlasLoadStage.ItemsQueued:
                        total = evt.Total;
                        break;
                    case AtlasLoadStage.Completed:
                        completed++;
                        float fraction = (float)completed / total;
                        EditorUtility.DisplayProgressBar("Loading Game Configs", $"{evt.Key} ({completed}/{total})", fraction);
                        if (slowMotion)
                            Thread.Sleep(200);
                        break;
                }
            });
            Initialize(progress, clear: () => EditorUtility.ClearProgressBar());
        }

        [InitializeOnLoadMethod]
        public static void AutoLoadGameConfigs()
        {
            Initialize();
        }

        static void Initialize(IProgress<AtlasLoadEvent> progress = null, Action clear = null)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            try
            {
                ConfLoader.DirectAccessDemo();
                InitializeCfgIdDrawers(ConfigAtlas.Instance);
                Debug.Log("<archmage> CfgIdDrawerManager.Initialize succeeded.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"<archmage> CfgIdDrawerManager.Initialize failed: {ex}");
            }
            finally
            {
                clear?.Invoke();
            }
        }

        static void InitializeCfgIdDrawers(ConfigAtlas atlas)
        {
            // Initialize XxxCfgIdDrawer on demand
            HeroCfgIdDrawer.Initialize(atlas.HeroTable, v => $"{v} ({new HeroCfgId(v).Cfg.HeroName.Text})");
        }
    }
}
