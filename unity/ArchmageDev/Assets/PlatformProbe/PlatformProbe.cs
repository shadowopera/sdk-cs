using System;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Shadop.Archmage.Sdk;
using UnityEngine;

// Probes how Task.Run and the loader behave on the current player platform.
// The case to run comes from the page URL on WebGL (index.html?case=<name>) and from the launch
// intent's "case" extra on Android (am start ... -e case <name>).
// Every line starts with [Probe]; the last line is "[Probe] DONE".
public class PlatformProbe : MonoBehaviour
{
    const int MaxFrames = 600;

    int _mainThread;

    async Awaitable Start()
    {
        _mainThread = Environment.CurrentManagedThreadId;
        var name = GetCase();
        Log($"case={name} platform={Application.platform} mainThread={_mainThread}");

        // Let the first frame finish so that frame counts start from a steady state.
        await Awaitable.NextFrameAsync();

        try
        {
            switch (name)
            {
                case "taskrun":
                    await ProbeTaskRun();
                    break;
                case "sync-resources":
                    ProbeSyncResources();
                    break;
                case "async-resources":
                    await ProbeAsync(new UnityResourcesFS(), "StaticConfigs");
                    break;
                case "async-streaming":
                    await ProbeAsync(new UnityStreamingAssetsFS(), "StreamingConfigs");
                    break;
                case "async-addressables":
                    await ProbeAsync(new UnityAddressablesFS(), "Assets/Configs");
                    break;
                case "async-addressables-greedy":
                    await ProbeAsync(new UnityAddressablesGreedyFS(), "Assets/Configs");
                    break;
                case "streaming-override-missing":
                    await ProbeAsync(new UnityStreamingAssetsFS(), "StreamingConfigs", "NoSuchOverrides");
                    break;
                default:
                    Log($"RESULT unknown case: {name}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Log($"RESULT exception: {ex}");
        }

        Log("DONE");
    }

    // Does the delegate run inline, later on the main thread, on another thread, or never?
    async Awaitable ProbeTaskRun()
    {
        // Time.frameCount may be read only on the main thread, so the delegate records just its thread.
        var startFrame = Time.frameCount;
        var ranThread = -1;
        var task = Task.Run(() => Volatile.Write(ref ranThread, Environment.CurrentManagedThreadId));
        Log($"after Task.Run: ran={Volatile.Read(ref ranThread) >= 0} status={task.Status}");

        while (!task.IsCompleted && Time.frameCount - startFrame < MaxFrames)
            await Awaitable.NextFrameAsync();

        if (!task.IsCompleted)
        {
            Log($"RESULT never ran within {MaxFrames} frames, status={task.Status}");
            return;
        }
        Log($"RESULT ran on thread {ranThread} (main={ranThread == _mainThread}), " +
            $"completed after {Time.frameCount - startFrame} frames, status={task.Status}");

        // Does an await on a Task.Run task resume?
        startFrame = Time.frameCount;
        var awaitTask = AwaitTaskRun();
        while (!awaitTask.IsCompleted && Time.frameCount - startFrame < MaxFrames)
            await Awaitable.NextFrameAsync();
        Log(awaitTask.IsCompleted
            ? $"RESULT await Task.Run resumed after {Time.frameCount - startFrame} frames, status={awaitTask.Status}"
            : $"RESULT await Task.Run did not resume within {MaxFrames} frames");
    }

    static async Task AwaitTaskRun()
    {
        await Task.Run(() => Thread.SpinWait(10));
    }

    void ProbeSyncResources()
    {
        var atlas = new ConfigAtlas();
        Log("before LoadAtlas");
        Archmage.LoadAtlas("StaticConfigs/atlas.json", "StaticConfigs", atlas, NewOptions(new UnityResourcesFS()));
        Log($"RESULT LoadAtlas returned, heroes={atlas.HeroTable.Count}");
    }

    async Task ProbeAsync(IFS fs, string cfgRoot, string overrideRoot = null)
    {
        var atlas = new ConfigAtlas();
        var timingFS = new TimingFS(fs);
        var options = NewOptions(timingFS);
        if (overrideRoot != null)
            options.WithOverrideRoot(overrideRoot + "/1").WithOverrideRoot(overrideRoot + "/2");
        var startFrame = Time.frameCount;
        var task = Archmage.LoadAtlasAsync($"{cfgRoot}/atlas.json", cfgRoot, atlas, options);
        while (!task.IsCompleted && Time.frameCount - startFrame < MaxFrames)
            await Awaitable.NextFrameAsync();

        if (!task.IsCompleted)
        {
            Log($"RESULT LoadAtlasAsync did not complete within {MaxFrames} frames");
            return;
        }
        await task;
        Log($"RESULT LoadAtlasAsync completed after {Time.frameCount - startFrame} frames, heroes={atlas.HeroTable.Count}");
        foreach (var r in timingFS.Reads)
            Log($"read {r.Path}: start={r.Start - startFrame} end={r.End - startFrame}");
    }

    // Records the frame at which each read starts and the frame at which it completes.
    class TimingFS : IFS
    {
        readonly IFS _inner;

        public TimingFS(IFS inner) => _inner = inner;

        public System.Collections.Generic.List<(string Path, int Start, int End)> Reads { get; } = new();

        public byte[] ReadAllBytes(string path) => _inner.ReadAllBytes(path);

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            var start = Time.frameCount;
            try
            {
                return await _inner.ReadAllBytesAsync(path, cancellationToken);
            }
            finally
            {
                Reads.Add((path, start, Time.frameCount));
            }
        }

        public bool FileExists(string path) => _inner.FileExists(path);

        public bool DirectoryExists(string path) => _inner.DirectoryExists(path);
    }

    static AtlasOptions NewOptions(IFS fs)
    {
        return new AtlasOptions()
            .WithLogger(new UnityAtlasLogger())
            .WithJsonSettings(UnityJsonSettingsFactory.Create())
            .WithFS(fs)
            .WithVariant("balance", "hard");
    }

    static string GetCase()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var intent = activity.Call<AndroidJavaObject>("getIntent"))
            return intent.Call<string>("getStringExtra", "case") ?? "taskrun";
#else
        var url = Application.absoluteURL;
        var i = url.IndexOf("case=", StringComparison.Ordinal);
        if (i < 0)
            return "taskrun";
        var s = url.Substring(i + "case=".Length);
        var end = s.IndexOfAny(new[] { '&', '#' });
        return end < 0 ? s : s.Substring(0, end);
#endif
    }

    static void Log(string msg)
    {
        Debug.Log($"[Probe] frame={Time.frameCount} thread={Environment.CurrentManagedThreadId} {msg}");
    }
}
