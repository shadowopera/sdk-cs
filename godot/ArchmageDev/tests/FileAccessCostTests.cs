using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Godot;
using Shadop.Archmage.Sdk;

namespace ArchmageDev.Tests
{
    // Measures how many frames each way of loading the atlas takes with GodotFileAccessFS.
    // Run explicitly: scripts/godot-test.sh --filter FileAccessCostTests
    static class FileAccessCostTests
    {
        public record Measurement(string Loading, ulong Frames, TimeSpan Elapsed);

        /// <summary>
        /// The measurements of this run, for the summary.
        /// </summary>
        public static List<Measurement> Measurements { get; } = new();

        [GodotTest(Explicit = true)]
        public static async Task LoadModes()
        {
            var tree = (SceneTree)Engine.GetMainLoop();

            // Warm up JIT compilation and file reads.
            await Archmage.LoadAtlasAsync(LoadTests.AtlasFile, LoadTests.CfgRoot, new ConfigAtlas(), Options());

            // Stretch frames like a real game, so that each wait for the main thread costs up to one frame.
            // --headless does not limit the frame rate.
            void Burn() => Thread.Sleep(16);
            tree.ProcessFrame += Burn;
            try
            {
                await Measure(tree, "LoadAtlas", () =>
                {
                    Archmage.LoadAtlas(LoadTests.AtlasFile, LoadTests.CfgRoot, new ConfigAtlas(), Options());
                    return Task.CompletedTask;
                });
                await Measure(tree, "LoadAtlasAsync", () =>
                    Archmage.LoadAtlasAsync(LoadTests.AtlasFile, LoadTests.CfgRoot, new ConfigAtlas(), Options()));
                await Measure(tree, "LoadAtlasAsync MaxConcurrency=1", () =>
                    Archmage.LoadAtlasAsync(LoadTests.AtlasFile, LoadTests.CfgRoot, new ConfigAtlas(),
                        Options().WithMaxConcurrency(1)));
                await Measure(tree, "LoadAtlasAsync MainThreadParsing", () =>
                    Archmage.LoadAtlasAsync(LoadTests.AtlasFile, LoadTests.CfgRoot, new ConfigAtlas(),
                        Options().WithMainThreadParsing()));
                await Measure(tree, "LoadAtlasAsync workerThreadLoading", () =>
                    Archmage.LoadAtlasAsync(LoadTests.AtlasFile, LoadTests.CfgRoot, new ConfigAtlas(), Options(),
                        workerThreadLoading: true));
            }
            finally
            {
                tree.ProcessFrame -= Burn;
            }
        }

        static async Task Measure(SceneTree tree, string loading, Func<Task> load)
        {
            // Start each load at the beginning of a frame.
            await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);

            var frame = Engine.GetProcessFrames();
            var sw = Stopwatch.StartNew();
            await load();
            sw.Stop();

            var m = new Measurement(loading, Engine.GetProcessFrames() - frame, sw.Elapsed);
            Measurements.Add(m);
            GD.Print($"[FileAccessCost] loading=\"{m.Loading}\" frames={m.Frames} ms={m.Elapsed.TotalMilliseconds:F1}");
        }

        static AtlasOptions Options() => LoadTests.Options(new ThreadProbeLog());
    }
}
