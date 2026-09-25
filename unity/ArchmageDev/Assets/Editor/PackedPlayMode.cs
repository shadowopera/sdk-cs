#nullable enable

using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace ArchmageDev.Editor
{
    // Switches PlayMode tests to load Addressables from built bundles.
    // Invoked by scripts/unity-test.sh --packed via -executeMethod.
    public static class PackedPlayMode
    {
        // Temp is wiped when the editor quits, so the saved index lives in Library.
        const string SavedIndexFile = "Library/ArchmagePlayModeIndex.txt";

        // Builds Addressables content and selects the "Use Existing Build" play mode script.
        public static void Enter()
        {
            var settings = GetSettings();

            // Keep the index saved by an earlier run whose Exit never ran; it holds the developer's setting.
            if (!File.Exists(SavedIndexFile))
                File.WriteAllText(SavedIndexFile, settings.ActivePlayModeDataBuilderIndex.ToString());

            var packedIndex = settings.DataBuilders.FindIndex(b => b is BuildScriptPackedPlayMode);
            if (packedIndex < 0)
                throw new InvalidOperationException("BuildScriptPackedPlayMode is not registered in the Addressables settings.");

            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            if (!string.IsNullOrEmpty(result.Error))
                throw new InvalidOperationException($"Addressables content build failed: {result.Error}");

            settings.ActivePlayModeDataBuilderIndex = packedIndex;
            Debug.Log($"[PackedPlayMode] Content built; play mode script set to {settings.ActivePlayModeDataBuilder.Name}.");
        }

        // Restores the play mode script when the editor opens after an interrupted packed run.
        // Batch mode is left alone: unity-test.sh restores it there.
        [InitializeOnLoadMethod]
        static void RestoreInteractiveEditor()
        {
            if (Application.isBatchMode || !File.Exists(SavedIndexFile))
                return;

            // Addressables settings may not be loadable while the editor is still initializing.
            EditorApplication.delayCall += () =>
            {
                if (!File.Exists(SavedIndexFile))
                    return;
                Debug.LogWarning("[PackedPlayMode] A packed test run was interrupted; restoring the Addressables play mode script.");
                Exit();
            };
        }

        // Restores the play mode script saved by Enter.
        public static void Exit()
        {
            if (!File.Exists(SavedIndexFile))
                return;

            var settings = GetSettings();
            settings.ActivePlayModeDataBuilderIndex = int.Parse(File.ReadAllText(SavedIndexFile).Trim());
            File.Delete(SavedIndexFile);
            Debug.Log($"[PackedPlayMode] Play mode script restored to {settings.ActivePlayModeDataBuilder.Name}.");
        }

        static AddressableAssetSettings GetSettings()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                throw new InvalidOperationException("Addressables settings not found.");
            return settings;
        }
    }
}
