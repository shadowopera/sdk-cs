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
    // Builds the WebGL probe player. Invoked by scripts/webgl-probe.sh via -executeMethod with
    // -buildTarget WebGL; the active build target is switched back to macOS standalone afterwards.
    public static class WebGLProbeBuild
    {
        const string ScenePath = "Assets/WebGLProbe/WebGLProbe.unity";
        const string OutputDir = "Builds/WebGLProbe";

        public static void Build()
        {
            try
            {
                BuildPlayer();
            }
            finally
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
            }
        }

        static void BuildPlayer()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            new GameObject("WebGLProbe").AddComponent<WebGLProbe>();
            EditorSceneManager.SaveScene(scene, ScenePath);

            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult content);
            if (!string.IsNullOrEmpty(content.Error))
                throw new InvalidOperationException($"Addressables content build failed: {content.Error}");

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.Development,
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"WebGL build failed: {report.summary.result}");
            Debug.Log($"[WebGLProbeBuild] Built {OutputDir}");
        }
    }
}
