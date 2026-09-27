#nullable enable

using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ArchmageDev.Editor
{
    // Builds the platform probe player. Invoked by scripts/webgl-probe.sh and scripts/android-probe.sh via
    // -executeMethod with the matching -buildTarget; the active build target is switched back to macOS
    // standalone afterwards. The scripts restore the project settings the build modifies.
    public static class PlatformProbeBuild
    {
        const string ScenePath = "Assets/PlatformProbe/PlatformProbe.unity";
        const string OutputDir = "Builds/PlatformProbe";

        public static void BuildWebGL() => Build(BuildTarget.WebGL, $"{OutputDir}/WebGL");

        public static void BuildApk() => Build(BuildTarget.Android, $"{OutputDir}/probe.apk");

        public static void BuildAab() => Build(BuildTarget.Android, $"{OutputDir}/probe.aab", appBundle: true);

        // Split Application Binary moves StreamingAssets into a Play Asset Delivery install-time pack.
        public static void BuildAabSplit() =>
            Build(BuildTarget.Android, $"{OutputDir}/probe-split.aab", appBundle: true, split: true);

        static void Build(BuildTarget target, string location, bool appBundle = false, bool split = false)
        {
            try
            {
                EditorUserBuildSettings.buildAppBundle = appBundle;
                PlayerSettings.Android.splitApplicationBinary = split;
                BuildPlayer(target, location);
            }
            finally
            {
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
            }
        }

        static void BuildPlayer(BuildTarget target, string location)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            new GameObject("PlatformProbe").AddComponent<PlatformProbe>();
            EditorSceneManager.SaveScene(scene, ScenePath);

            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult content);
            if (!string.IsNullOrEmpty(content.Error))
                throw new InvalidOperationException($"Addressables content build failed: {content.Error}");

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = location,
                target = target,
                options = BuildOptions.Development,
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"{target} build failed: {report.summary.result}");
            Debug.Log($"[PlatformProbeBuild] Built {location}");
        }
    }
}
