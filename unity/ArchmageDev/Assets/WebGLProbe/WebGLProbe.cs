using System;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Shadop.Archmage.Sdk;
using UnityEngine;

// Probes how Task.Run and the loader behave on the current player platform.
// The case to run is taken from the page URL: index.html?case=<name>.
// Every line starts with [Probe]; the last line is "[Probe] DONE".
public class WebGLProbe : MonoBehaviour
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
                case "taskrun-wait":
                    ProbeTaskRunWait();
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
        var startFrame = Time.frameCount;
        var ranFrame = -1;
        var ranThread = -1;
        var task = Task.Run(() =>
        {
            ranFrame = Time.frameCount;
            ranThread = Environment.CurrentManagedThreadId;
        });
        Log($"after Task.Run: ran={ranFrame >= 0} status={task.Status}");

        while (!task.IsCompleted && Time.frameCount - startFrame < MaxFrames)
            await Awaitable.NextFrameAsync();

        if (!task.IsCompleted)
        {
            Log($"RESULT never ran within {MaxFrames} frames, status={task.Status}");
            return;
        }
        Log($"RESULT ran after {ranFrame - startFrame} frames on thread {ranThread} (main={ranThread == _mainThread}), " +
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

    // Blocks the main thread on a Task.Run task, as the synchronous loader does.
    void ProbeTaskRunWait()
    {
        var task = Task.Run(() => Thread.SpinWait(10));
        Log("before Task.WaitAll");
        Task.WaitAll(task);
        Log($"RESULT Task.WaitAll returned, status={task.Status}");
    }

    void ProbeSyncResources()
    {
        var atlas = new ConfigAtlas();
        Log("before LoadAtlas");
        Archmage.LoadAtlas("StaticConfigs/atlas.json", "StaticConfigs", atlas, NewOptions(new UnityResourcesFS()));
        Log($"RESULT LoadAtlas returned, heroes={atlas.HeroTable.Count}");
    }

    async Task ProbeAsync(IFS fs, string cfgRoot)
    {
        var atlas = new ConfigAtlas();
        var startFrame = Time.frameCount;
        var task = Archmage.LoadAtlasAsync($"{cfgRoot}/atlas.json", cfgRoot, atlas, NewOptions(fs));
        while (!task.IsCompleted && Time.frameCount - startFrame < MaxFrames)
            await Awaitable.NextFrameAsync();

        if (!task.IsCompleted)
        {
            Log($"RESULT LoadAtlasAsync did not complete within {MaxFrames} frames");
            return;
        }
        await task;
        Log($"RESULT LoadAtlasAsync completed after {Time.frameCount - startFrame} frames, heroes={atlas.HeroTable.Count}");
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
        var url = Application.absoluteURL;
        var i = url.IndexOf("case=", StringComparison.Ordinal);
        if (i < 0)
            return "taskrun";
        var s = url.Substring(i + "case=".Length);
        var end = s.IndexOfAny(new[] { '&', '#' });
        return end < 0 ? s : s.Substring(0, end);
    }

    void Log(string msg)
    {
        Debug.Log($"[Probe] frame={Time.frameCount} thread={Environment.CurrentManagedThreadId} {msg}");
    }
}
