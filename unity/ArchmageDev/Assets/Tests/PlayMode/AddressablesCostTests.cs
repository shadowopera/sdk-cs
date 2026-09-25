// AssetDatabaseProvider only exists in the editor.
#if UNITY_EDITOR

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Conf;
using Shadop.Archmage.Sdk;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using GameObject = UnityEngine.GameObject;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
using Time = UnityEngine.Time;

// Measures how many frames missing override files cost with UnityAddressablesFS.
// Run explicitly: scripts/unity-test.sh --filter AddressablesCostTests
[Explicit("Measurement only")]
public class AddressablesCostTests
{
    // Records which thread each read starts on.
    class ProbeFS : IFS
    {
        readonly IFS _inner = new UnityAddressablesFS();
        readonly int _mainThreadId;
        int _mainReads;
        int _workerReads;

        public ProbeFS(int mainThreadId) => _mainThreadId = mainThreadId;

        public int MainReads => _mainReads;
        public int WorkerReads => _workerReads;

        public byte[] ReadAllBytes(string path) => _inner.ReadAllBytes(path);

        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
                Interlocked.Increment(ref _mainReads);
            else
                Interlocked.Increment(ref _workerReads);
            return _inner.ReadAllBytesAsync(path, cancellationToken);
        }

        public bool FileExists(string path) => _inner.FileExists(path);
        public bool DirectoryExists(string path) => _inner.DirectoryExists(path);

        public Task PrepareAsync(System.Collections.Generic.IReadOnlyCollection<string> paths,
            CancellationToken cancellationToken = default) => _inner.PrepareAsync(paths, cancellationToken);
    }

    // Stretches each frame to about 16 ms. Application.targetFrameRate has no effect in batch mode.
    class FrameTimeBurner : MonoBehaviour
    {
        void Update() => Thread.Sleep(16);
    }

    [Test]
    public async Task MissingOverrides()
    {
        // Warm up Addressables initialization and asset loading.
        await Measure(false, 0, "warmup");

        // "Use Asset Database" registers AssetDatabaseProvider; "Use Existing Build" loads bundles instead.
        var providers = Addressables.ResourceManager.ResourceProviders.OfType<AssetDatabaseProvider>().ToList();

        // Stretch frames like a real game, so that each main thread hop costs up to one frame.
        var burner = new GameObject("FrameTimeBurner", typeof(FrameTimeBurner));
        try
        {
            if (providers.Count == 0)
            {
                await MeasureAll("mode=packed");
                return;
            }

            // A load delay of 0 completes asset loads synchronously; 0.02 s makes them take
            // at least one frame, like loading from asset bundles. The provider treats delays
            // below 0.01 s as 0.
            foreach (var loadDelay in new[] { 0f, 0.02f })
            {
                foreach (var provider in providers)
                    provider.SetLoadDelay(loadDelay);
                await MeasureAll($"mode=assetdb loadDelay={loadDelay}");
            }
        }
        finally
        {
            Object.Destroy(burner);
        }
    }

    static async Task MeasureAll(string label)
    {
        foreach (var concurrent in new[] { false, true })
        {
            foreach (var roots in new[] { 0, 1, 4 })
                await Measure(concurrent, roots, label);
        }
    }

    static async Task Measure(bool concurrent, int missingRoots, string label)
    {
        // Let Addressables run deferred completion callbacks, which release the operations
        // of the previous load; otherwise this load hits them in the operation cache.
        await UnityEngine.Awaitable.NextFrameAsync();

        var fs = new ProbeFS(Thread.CurrentThread.ManagedThreadId);
        var options = new AtlasOptions()
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(fs)
            .WithVariant("balance", "hard");
        for (var i = 0; i < missingRoots; i++)
            options.WithOverrideRoot($"Assets/ConfigOverrides/missing{i}");
        if (concurrent)
            options.WithAsyncLoadStrategy(async (items, loadAsync, ct) =>
            {
                await Task.WhenAll(items.Select(kvp => loadAsync(kvp.Key, kvp.Value, ct)));
            });

        var frame = Time.frameCount;
        var sw = Stopwatch.StartNew();
        await Archmage.LoadAtlasAsync("Assets/Configs/atlas.json", "Assets/Configs", new ConfigAtlas(), options);
        sw.Stop();

        Debug.Log($"[AddressablesCost] {label} concurrent={concurrent} missingRoots={missingRoots} " +
                  $"frames={Time.frameCount - frame} ms={sw.Elapsed.TotalMilliseconds:F1} " +
                  $"reads(main={fs.MainReads}, worker={fs.WorkerReads})");
    }
}

#endif
