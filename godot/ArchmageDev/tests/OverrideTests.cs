using System.Collections.Generic;
using System.Threading.Tasks;
using Conf;
using Godot;
using Shadop.Archmage.Sdk;

namespace ArchmageDev.Tests
{
    // Loads the atlas from res://configs with two override roots. Root 2 lacks some files that root 1 has, so the
    // loader must skip missing override files in every location.
    static class OverrideTests
    {
        const string OverrideRoot = "res://config_overrides";

        [GodotTest]
        public static async Task ResOverrides()
        {
            await LoadAndAssert(OverrideRoot);
        }

        [GodotTest]
        public static async Task UserOverrides()
        {
            const string userRoot = "user://config_overrides";
            RemoveDir(userRoot);
            foreach (var file in ListFiles(OverrideRoot))
            {
                var target = userRoot + file.Substring(OverrideRoot.Length);
                Assert.AreEqual(Error.Ok, DirAccess.MakeDirRecursiveAbsolute(target.GetBaseDir()), $"Cannot create {target.GetBaseDir()}.");
                Assert.AreEqual(Error.Ok, DirAccess.CopyAbsolute(file, target), $"Cannot copy {file} to {target}.");
            }

            await LoadAndAssert(userRoot);
        }

        [GodotTest]
        public static async Task ResourcePackOverrides()
        {
            // Godot cannot unmount a resource pack, so the pack uses its own paths instead of replacing files that
            // later tests read.
            const string packFile = "user://overrides.pck";
            const string packRoot = "res://pack_overrides";
            var packer = new PckPacker();
            Assert.AreEqual(Error.Ok, packer.PckStart(packFile), $"Cannot start {packFile}.");
            foreach (var file in ListFiles(OverrideRoot))
            {
                var target = packRoot + file.Substring(OverrideRoot.Length);
                Assert.AreEqual(Error.Ok, packer.AddFile(target, file), $"Cannot add {file} to {packFile}.");
            }
            Assert.AreEqual(Error.Ok, packer.Flush(), $"Cannot write {packFile}.");
            Assert.IsTrue(ProjectSettings.LoadResourcePack(packFile), $"Cannot mount {packFile}.");

            await LoadAndAssert(packRoot);
        }

        static async Task LoadAndAssert(string overrideRoot)
        {
            var atlas = new ConfigAtlas();
            var log = new ThreadProbeLog();
            await Archmage.LoadAtlasAsync(LoadTests.AtlasFile, LoadTests.CfgRoot, atlas,
                LoadTests.Options(log)
                    .WithOverrideRoot($"{overrideRoot}/1")
                    .WithOverrideRoot($"{overrideRoot}/2"),
                progress: log);
            AssertOverrides(atlas, log);
        }

        // The same checks as AssertOverrides in the Unity OverrideTests, with Godot vectors.
        static void AssertOverrides(ConfigAtlas atlas, ThreadProbeLog log)
        {
            // Overrides applied per key: game and skill come from both roots, hero and item from root 1 only.
            Assert.AreEqual(2, log.CountEvents("game", AtlasLoadStage.ApplyingOverride));
            Assert.AreEqual(2, log.CountEvents("skill", AtlasLoadStage.ApplyingOverride));
            Assert.AreEqual(1, log.CountEvents("hero", AtlasLoadStage.ApplyingOverride));
            Assert.AreEqual(1, log.CountEvents("item", AtlasLoadStage.ApplyingOverride));

            // Root 2 wins over root 1.
            Assert.AreEqual(80, atlas.GameCfg.MaxLevel);
            Assert.AreEqual(30, atlas.SkillTable["slash"].Damage.Max);

            // Fields set by a single root.
            Assert.AreEqual(new Vector3(0, 20, -7), atlas.GameCfg.CameraOffset);
            Assert.AreEqual(new Vector4(1, 0.5f, 0.25f, 0.5f), atlas.GameCfg.Tint);
            Assert.AreEqual(130f, atlas.HeroTable[2].Stats!.Atk);
            Assert.AreEqual(new ItemCfgId(104), atlas.HeroTable[3].Weapon.CfgId);
        }

        static List<string> ListFiles(string dir)
        {
            var files = new List<string>();
            foreach (var name in DirAccess.GetFilesAt(dir))
                files.Add($"{dir}/{name}");
            foreach (var name in DirAccess.GetDirectoriesAt(dir))
                files.AddRange(ListFiles($"{dir}/{name}"));
            Assert.IsTrue(files.Count > 0, $"No files found in {dir}.");
            return files;
        }

        static void RemoveDir(string dir)
        {
            if (!DirAccess.DirExistsAbsolute(dir))
                return;
            foreach (var name in DirAccess.GetDirectoriesAt(dir))
                RemoveDir($"{dir}/{name}");
            foreach (var name in DirAccess.GetFilesAt(dir))
                Assert.AreEqual(Error.Ok, DirAccess.RemoveAbsolute($"{dir}/{name}"), $"Cannot remove {dir}/{name}.");
            Assert.AreEqual(Error.Ok, DirAccess.RemoveAbsolute(dir), $"Cannot remove {dir}.");
        }
    }
}
