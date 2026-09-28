using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    /// <summary>
    /// Wraps DefaultFS, records the threads IFS is called on and tracks reads in flight. Each read takes at least
    /// 5 ms, so that concurrent reads overlap.
    /// </summary>
    class ProbeFS : IFS
    {
        readonly IFS _inner = new DefaultFS();
        readonly string? _failPath;
        readonly Func<string, Exception> _failure;
        int _inFlight;
        int _maxInFlight;

        /// <param name="failPath">Reading a path ending with this throws the exception made by failure.</param>
        /// <param name="failure">Makes the exception from the path. Defaults to IOException.</param>
        public ProbeFS(string? failPath = null, Func<string, Exception>? failure = null)
        {
            _failPath = failPath;
            _failure = failure ?? (path => new IOException($"Injected failure: {path}"));
        }

        public bool MainThreadOnly { get; init; }
        public ConcurrentBag<int> ThreadIds { get; } = new();
        public ConcurrentBag<(string Path, int ThreadId)> SyncReads { get; } = new();
        public ConcurrentBag<string> AsyncReads { get; } = new();
        public int InFlight => Volatile.Read(ref _inFlight);
        public int MaxInFlight => Volatile.Read(ref _maxInFlight);

        public byte[] ReadAllBytes(string path)
        {
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            SyncReads.Add((path, Environment.CurrentManagedThreadId));
            EnterRead();
            try
            {
                Thread.Sleep(5);
                ThrowIfFailPath(path);
                return _inner.ReadAllBytes(path);
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            AsyncReads.Add(path);
            EnterRead();
            try
            {
                await Task.Delay(5, cancellationToken);
                ThrowIfFailPath(path);
                return await _inner.ReadAllBytesAsync(path, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
        }

        void EnterRead()
        {
            var n = Interlocked.Increment(ref _inFlight);
            int max;
            while (n > (max = Volatile.Read(ref _maxInFlight)))
                Interlocked.CompareExchange(ref _maxInFlight, n, max);
        }

        void ThrowIfFailPath(string path)
        {
            if (_failPath is not null && path.Replace('\\', '/').EndsWith(_failPath))
                throw _failure(path);
        }

        public bool FileExists(string path)
        {
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            return _inner.FileExists(path);
        }

        public bool DirectoryExists(string path)
        {
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            return _inner.DirectoryExists(path);
        }
    }

    /// <summary>
    /// Runs async code on a dedicated thread whose SynchronizationContext posts back to it,
    /// like the Unity main thread.
    /// </summary>
    sealed class SingleThreadContext : SynchronizationContext
    {
        readonly BlockingCollection<(SendOrPostCallback, object?)> _queue = new();

        public int ThreadId { get; private set; }

        public override void Post(SendOrPostCallback d, object? state) => _queue.Add((d, state));

        public override void Send(SendOrPostCallback d, object? state) => throw new NotSupportedException();

        public static int Run(Func<Task> func)
        {
            var ctx = new SingleThreadContext();
            Exception? error = null;
            var thread = new Thread(() =>
            {
                ctx.ThreadId = Environment.CurrentManagedThreadId;
                SetSynchronizationContext(ctx);
                var task = func();
                task.ContinueWith(_ => ctx._queue.CompleteAdding(), TaskScheduler.Default);
                foreach (var (d, state) in ctx._queue.GetConsumingEnumerable())
                    d(state);
                error = task.Exception?.InnerException;
            })
            { IsBackground = true };
            thread.Start();
            if (!thread.Join(TimeSpan.FromSeconds(30)))
                throw new TimeoutException("Deadlocked on the SynchronizationContext thread.");
            if (error is not null)
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
            return ctx.ThreadId;
        }
    }

    /// <summary>
    /// Wraps ConfigAtlas and records the threads BindRefs and OnLoaded are called on.
    /// </summary>
    sealed class ThreadProbeAtlas : IAtlas
    {
        public ConfigAtlas Inner { get; } = new();
        public int BindRefsThreadId { get; private set; }
        public int OnLoadedThreadId { get; private set; }

        public void SetDataVersion(VersionInfo? v) => Inner.SetDataVersion(v);

        public Dictionary<string, AtlasItem> AtlasItems() => Inner.AtlasItems();

        public void BindRefs()
        {
            BindRefsThreadId = Environment.CurrentManagedThreadId;
            Inner.BindRefs();
        }

        public void OnLoaded()
        {
            OnLoadedThreadId = Environment.CurrentManagedThreadId;
            Inner.OnLoaded();
        }
    }

    public partial class AtlasTests
    {
        const string AtlasFile = "../../../testdata/atlas.json";
        [Theory]
        [InlineData(false, 1)]
        [InlineData(false, 32)]
        [InlineData(true, 1)]
        [InlineData(true, 3)]
        [InlineData(true, 32)]
        public async Task TestAtlas_WithMaxConcurrency(bool isAsync, int n)
        {
            var fs = new ProbeFS();
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" })
                .WithMaxConcurrency(n);

            var atlas = new ConfigAtlas();
            var events = new ConcurrentBag<AtlasLoadEvent>();
            var progress = new SyncProgress<AtlasLoadEvent>(events.Add);

            if (isAsync)
                await Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata",
                    atlas, opts, progress: progress, cancellationToken: TestContext.Current.CancellationToken);
            else
                Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts, progress);
            CheckUpdateGolden(atlas, "../../../golden/max_concurrency");

            // The largest item ("skill") has 3 files.
            if (isAsync)
                Assert.InRange(fs.MaxInFlight, 1, n * 3);

            var skillEvents = events.Where(e => e.Key == "skill").ToList();
            Assert.Equal(1, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartProcessing));
            Assert.Equal(3, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartReading));
            Assert.Equal(3, skillEvents.Count(e => e.Stage == AtlasLoadStage.StartParsing));
            Assert.Equal(1, skillEvents.Count(e => e.Stage == AtlasLoadStage.Completed));
        }

        [Fact]
        public void TestAtlas_WithMaxConcurrency_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AtlasOptions().WithMaxConcurrency(0));
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(32, true)]
        [InlineData(32, false)]
        public void TestAtlas_LoadAtlasAsync_IFSOnCallerContext(int n, bool mainFSMainThreadOnly)
        {
            // One IFS that works only on the main thread keeps every IFS on the caller's context.
            var fs = new ProbeFS { MainThreadOnly = mainFSMainThreadOnly };
            var ovrFS = new ProbeFS { MainThreadOnly = true };
            var threadId = SingleThreadContext.Run(() => Archmage.LoadAtlasAsync(
                "../../../testdata/atlas.json", "../../../testdata", new ConfigAtlas(),
                DefaultOpts()
                    .WithLogger(new ScavengerLogger())
                    .WithFS(fs)
                    .WithBlacklist(new[] { "balance" })
                    .WithOverrideFS(ovrFS, "../../../override/1")
                    .WithMaxConcurrency(n)));

            Assert.NotEmpty(fs.ThreadIds);
            Assert.All(fs.ThreadIds, id => Assert.Equal(threadId, id));
            Assert.NotEmpty(ovrFS.ThreadIds);
            Assert.All(ovrFS.ThreadIds, id => Assert.Equal(threadId, id));
            Assert.Empty(fs.SyncReads);
            Assert.Empty(ovrFS.SyncReads);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestAtlas_IFSOnAnyThread_ReadsItemsOnThreadPool(bool isAsync)
        {
            var fs = new ProbeFS();
            var ovrFS = new ProbeFS();
            var atlas = new ConfigAtlas();
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" })
                .WithOverrideFS(ovrFS, "../../../override/1")
                .WithOverrideFS(ovrFS, "../../../override/2");

            var threadId = SingleThreadContext.Run(() =>
            {
                if (isAsync)
                    return Archmage.LoadAtlasAsync(AtlasFile, "../../../testdata", atlas, opts);
                Archmage.LoadAtlas(AtlasFile, "../../../testdata", atlas, opts);
                return Task.CompletedTask;
            });
            CheckUpdateGolden(atlas, "../../../golden/override_root");

            // Items are read with ReadAllBytes on thread pool threads, several at a time.
            var itemReads = fs.SyncReads.Where(r => r.Path != AtlasFile).Concat(ovrFS.SyncReads).ToList();
            Assert.NotEmpty(itemReads);
            Assert.All(itemReads, r => Assert.NotEqual(threadId, r.ThreadId));
            Assert.InRange(fs.MaxInFlight, 2, int.MaxValue);

            // LoadAtlasAsync reads only atlas.json with ReadAllBytesAsync, on the caller's context.
            Assert.Equal(isAsync ? new[] { AtlasFile } : Array.Empty<string>(), fs.AsyncReads);
            Assert.Empty(ovrFS.AsyncReads);
        }

        [Fact]
        public void TestAtlas_LoadAtlas_WithSynchronizationContext()
        {
            // Sync loading blocks a thread that owns a SynchronizationContext, like the Unity main thread.
            // It must neither deadlock nor call IFS on another thread.
            var fs = new ProbeFS { MainThreadOnly = true };
            var ovrFS = new ProbeFS { MainThreadOnly = true };
            var threadId = SingleThreadContext.Run(() =>
            {
                Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", new ConfigAtlas(),
                    DefaultOpts()
                        .WithLogger(new ScavengerLogger())
                        .WithFS(fs)
                        .WithBlacklist(new[] { "balance" })
                        .WithOverrideFS(ovrFS, "../../../override/1"));
                return Task.CompletedTask;
            });

            Assert.NotEmpty(fs.ThreadIds);
            Assert.All(fs.ThreadIds, id => Assert.Equal(threadId, id));
            Assert.All(ovrFS.ThreadIds, id => Assert.Equal(threadId, id));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void TestAtlas_WithMainThreadParsing(bool isAsync, bool mainThreadOnly)
        {
            // Reading and parsing must stay on the caller's thread: StartParsing is reported from the parse step.
            var fs = new ProbeFS { MainThreadOnly = mainThreadOnly };
            var atlas = new ConfigAtlas();
            var parseThreadIds = new ConcurrentBag<int>();
            var progress = new SyncProgress<AtlasLoadEvent>(e =>
            {
                if (e.Stage == AtlasLoadStage.StartParsing)
                    parseThreadIds.Add(Environment.CurrentManagedThreadId);
            });
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" })
                .WithMainThreadParsing();

            var threadId = SingleThreadContext.Run(() =>
            {
                if (isAsync)
                    return Archmage.LoadAtlasAsync("../../../testdata/atlas.json", "../../../testdata",
                        atlas, opts, progress: progress);
                Archmage.LoadAtlas("../../../testdata/atlas.json", "../../../testdata", atlas, opts, progress);
                return Task.CompletedTask;
            });
            CheckUpdateGolden(atlas, "../../../golden/max_concurrency");

            Assert.NotEmpty(parseThreadIds);
            Assert.All(parseThreadIds, id => Assert.Equal(threadId, id));
            Assert.All(fs.ThreadIds, id => Assert.Equal(threadId, id));

            // An IFS that can be called on any thread may implement ReadAllBytesAsync with the thread pool.
            if (!mainThreadOnly)
                Assert.Empty(fs.AsyncReads);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task TestAtlas_FailureWaitsForInFlight(bool isAsync, bool mainThreadOnly)
        {
            var fs = new ProbeFS(failPath: "vtbl/skill-magic.json") { MainThreadOnly = mainThreadOnly };
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" });

            var err = await Assert.ThrowsAsync<ArchmageException>(async () =>
            {
                if (isAsync)
                    await Archmage.LoadAtlasAsync(AtlasFile, "../../../testdata", new ConfigAtlas(), opts,
                        cancellationToken: TestContext.Current.CancellationToken);
                else
                    Archmage.LoadAtlas(AtlasFile, "../../../testdata", new ConfigAtlas(), opts);
            });
            Assert.StartsWith("<archmage> Failed to load atlas item: \"skill\"", err.Message);
            Assert.IsType<IOException>(err.InnerException);

            // No read outlives the load.
            Assert.Equal(0, fs.InFlight);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestAtlas_LoadAtlasAsync_IFSCanceledIsFailure(bool mainThreadOnly)
        {
            // The caller did not cancel, so a TaskCanceledException from IFS (e.g. a timeout) fails the item.
            var fs = new ProbeFS(failPath: "vtbl/skill-magic.json", failure: _ => new TaskCanceledException("Timeout"))
            {
                MainThreadOnly = mainThreadOnly
            };
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" });

            var err = await Assert.ThrowsAsync<ArchmageException>(() => Archmage.LoadAtlasAsync(
                "../../../testdata/atlas.json", "../../../testdata", new ConfigAtlas(), opts,
                cancellationToken: TestContext.Current.CancellationToken));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"skill\"", err.Message);
            Assert.IsType<TaskCanceledException>(err.InnerException);
            Assert.Equal(0, fs.InFlight);
        }

        [Fact]
        public void TestAtlas_WorkerThreadLoading()
        {
            var fs = new ProbeFS();
            var ovrFS = new ProbeFS();
            var atlas = new ThreadProbeAtlas();
            var progressThreadIds = new ConcurrentBag<int>();
            var progress = new SyncProgress<AtlasLoadEvent>(_ => progressThreadIds.Add(Environment.CurrentManagedThreadId));
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" })
                .WithOverrideFS(ovrFS, "../../../override/1")
                .WithOverrideFS(ovrFS, "../../../override/2");

            var threadId = SingleThreadContext.Run(() => Archmage.LoadAtlasAsync(
                AtlasFile, "../../../testdata", atlas, opts, workerThreadLoading: true, progress: progress));
            CheckUpdateGolden(atlas.Inner, "../../../golden/override_root");

            // Only OnLoaded runs on the caller's context.
            Assert.Equal(threadId, atlas.OnLoadedThreadId);
            Assert.NotEqual(threadId, atlas.BindRefsThreadId);
            Assert.NotEmpty(fs.ThreadIds);
            Assert.All(fs.ThreadIds.Concat(ovrFS.ThreadIds), id => Assert.NotEqual(threadId, id));
            Assert.NotEmpty(progressThreadIds);
            Assert.All(progressThreadIds, id => Assert.NotEqual(threadId, id));

            // atlas.json is read with ReadAllBytes too.
            Assert.Empty(fs.AsyncReads);
            Assert.Empty(ovrFS.AsyncReads);
        }

        [Theory]
        [InlineData("main IFS")]
        [InlineData("override IFS")]
        [InlineData("MainThreadParsing")]
        public async Task TestAtlas_WorkerThreadLoading_Rejected(string cause)
        {
            var fs = new ProbeFS { MainThreadOnly = cause == "main IFS" };
            var ovrFS = new ProbeFS { MainThreadOnly = cause == "override IFS" };
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithOverrideFS(ovrFS, "../../../override/1");
            if (cause == "MainThreadParsing")
                opts.WithMainThreadParsing();

            var err = await Assert.ThrowsAsync<ArchmageException>(() => Archmage.LoadAtlasAsync(
                AtlasFile, "../../../testdata", new ConfigAtlas(), opts, workerThreadLoading: true,
                cancellationToken: TestContext.Current.CancellationToken));
            Assert.StartsWith("<archmage> workerThreadLoading cannot be used", err.Message);

            // Rejected before any IFS call.
            Assert.Empty(fs.ThreadIds);
            Assert.Empty(ovrFS.ThreadIds);
        }
    }
}
