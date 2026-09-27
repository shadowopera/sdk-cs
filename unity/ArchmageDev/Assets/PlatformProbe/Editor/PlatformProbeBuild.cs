#nullable enable

using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
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
            AddBulkFiles();

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

        // Adds PlatformProbe.BulkCount small config files to the default Addressables group, and as many
        // files of about 50KB to a new group, each set as one folder entry.
        static void AddBulkFiles()
        {
            var random = new System.Random(1);
            WriteBulkFiles(PlatformProbe.BulkDir, 10, random);
            WriteBulkFiles(PlatformProbe.LargeDir, 500, random);
            AssetDatabase.Refresh();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddFolderEntry(settings, PlatformProbe.BulkDir, settings.DefaultGroup);
            var largeGroup = settings.CreateGroup("Probe Large", false, false, true, new System.Collections.Generic.List<AddressableAssetGroupSchema>(settings.DefaultGroup.Schemas));
            AddFolderEntry(settings, PlatformProbe.LargeDir, largeGroup);
            AssetDatabase.SaveAssets();
        }

        static void WriteBulkFiles(string dir, int rowsPerFile, System.Random random)
        {
            Directory.CreateDirectory(dir);
            for (var i = 0; i < PlatformProbe.BulkCount; i++)
            {
                var rows = new System.Text.StringBuilder("{");
                for (var j = 0; j < rowsPerFile; j++)
                {
                    if (j > 0)
                        rows.Append(',');
                    var id = i * rowsPerFile + j;
                    rows.Append($"\"{id}\":{{\"id\":{id},\"name\":\"row {random.Next():x8}\",\"hp\":{random.Next(1000)}," +
                        $"\"atk\":{random.Next(100)},\"desc\":\"{random.Next():x8}{random.Next():x8}{random.Next():x8}\"}}");
                }
                File.WriteAllText(PlatformProbe.BulkFile(dir, i), rows.Append('}').ToString());
            }
        }

        static void AddFolderEntry(AddressableAssetSettings settings, string dir, AddressableAssetGroup group)
        {
            var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(dir), group);
            entry.address = dir;
        }
    }
}
