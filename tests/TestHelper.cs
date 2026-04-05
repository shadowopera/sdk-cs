using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Shadop.Archmage.Sdk;

namespace Shadop.Archmage.Sdk.Tests
{
    class ScavengerLogger : IAtlasLogger
    {
        readonly object _lock = new();
        public List<string> Lines { get; } = new();

        public void Info(string message)
        {
            lock (_lock)
            {
                Lines.Add($"INF {message}");
            }
        }
    }

    class MemoryFS : IFS
    {
        readonly Dictionary<string, byte[]> _fsys;

        public MemoryFS(Dictionary<string, byte[]> fsys)
        {
            _fsys = fsys;
        }

        public bool DirectoryExists(string path)
        {
            return true;
        }

        public bool FileExists(string path)
        {
            var key = path.Replace('\\', '/');
            return _fsys.ContainsKey(key);
        }

        public byte[] ReadAllBytes(string path)
        {
            var key = path.Replace('\\', '/');
            if (_fsys.TryGetValue(key, out var data)) return data;
            throw new FileNotFoundException(path);
        }

        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            var key = path.Replace('\\', '/');
            if (!_fsys.TryGetValue(key, out var data)) throw new FileNotFoundException(path);
            return FunctionInside();

            async Task<byte[]> FunctionInside()
            {
                await Task.Yield();
                return data;
            }
        }
    }

    class SyncProgress<T> : IProgress<T>
    {
        readonly Action<T> _handler;
        public SyncProgress(Action<T> handler) => _handler = handler;
        public void Report(T value) => _handler(value);
    }

    public partial class AtlasTests
    {
        internal static AtlasOptions DefaultOpts()
        {
            return new AtlasOptions().WithJsonSettings(Archmage.CreateJsonDumpSettings());
        }

        internal static void CheckUpdateGolden(IAtlas atlas, string goldenDir)
        {
            var updateGolden = Environment.GetEnvironmentVariable("UPDATE_GOLDEN") == "1";
            if (updateGolden)
            {
                Archmage.DumpAtlas(atlas, goldenDir, Archmage.CreateJsonDumpSettings());
            }
            else
            {
                var settings = Archmage.CreateJsonDumpSettings();
                foreach (var kvp in System.Linq.Enumerable.OrderBy(atlas.AtlasItems(), k => k.Key))
                {
                    if (!kvp.Value.Ready || kvp.Value.Cfg == null) continue;

                    var actualJson = Newtonsoft.Json.JsonConvert.SerializeObject(kvp.Value.Cfg, settings) + "\n";
                    var goldenFile = Path.Combine(goldenDir, kvp.Key + ".json");

                    Xunit.Assert.True(File.Exists(goldenFile), $"Golden file {goldenFile} does not exist. Run tests with UPDATE_GOLDEN=1 to generate.");

                    var expectedJson = File.ReadAllText(goldenFile);

                    // Normalize line endings for cross-platform comparison
                    expectedJson = expectedJson.Replace("\r\n", "\n");
                    actualJson = actualJson.Replace("\r\n", "\n");

                    Xunit.Assert.Equal(expectedJson, actualJson);
                }
            }
        }
    }
}
