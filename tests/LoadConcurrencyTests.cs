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
    /// Wraps DefaultFS, records the threads IFS is called on and tracks reads in flight.
    /// </summary>
    class ProbeFS : IFS
    {
        readonly IFS _inner = new DefaultFS();
        readonly string? _failPath;
        int _inFlight;
        int _maxInFlight;

        /// <param name="failPath">Reading a path ending with this throws IOException.</param>
        public ProbeFS(string? failPath = null)
        {
            _failPath = failPath;
        }

        public ConcurrentBag<int> ThreadIds { get; } = new();
        public int InFlight => Volatile.Read(ref _inFlight);
        public int MaxInFlight => Volatile.Read(ref _maxInFlight);

        public byte[] ReadAllBytes(string path)
        {
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            return _inner.ReadAllBytes(path);
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            ThreadIds.Add(Environment.CurrentManagedThreadId);
            var n = Interlocked.Increment(ref _inFlight);
            int max;
            while (n > (max = Volatile.Read(ref _maxInFlight)))
                Interlocked.CompareExchange(ref _maxInFlight, n, max);
            try
            {
                await Task.Delay(5, cancellationToken);
                if (_failPath is not null && path.Replace('\\', '/').EndsWith(_failPath))
                    throw new IOException($"Injected failure: {path}");
                return await _inner.ReadAllBytesAsync(path, cancellationToken);
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
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

    public partial class AtlasTests
    {
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
                    atlas, opts, progress, cancellationToken: TestContext.Current.CancellationToken);
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
        [InlineData(1)]
        [InlineData(32)]
        public void TestAtlas_LoadAtlasAsync_IFSOnCallerContext(int n)
        {
            var fs = new ProbeFS();
            var ovrFS = new ProbeFS();
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
        }

        [Fact]
        public void TestAtlas_LoadAtlas_WithSynchronizationContext()
        {
            // Sync loading blocks a thread that owns a SynchronizationContext, like the Unity main thread.
            // It must neither deadlock nor call IFS on another thread.
            var fs = new ProbeFS();
            var ovrFS = new ProbeFS();
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

        [Fact]
        public async Task TestAtlas_LoadAtlasAsync_FailureWaitsForInFlight()
        {
            var fs = new ProbeFS(failPath: "vtbl/skill-magic.json");
            var opts = DefaultOpts()
                .WithLogger(new ScavengerLogger())
                .WithFS(fs)
                .WithBlacklist(new[] { "balance" });

            var err = await Assert.ThrowsAsync<ArchmageException>(() => Archmage.LoadAtlasAsync(
                "../../../testdata/atlas.json", "../../../testdata", new ConfigAtlas(), opts,
                cancellationToken: TestContext.Current.CancellationToken));
            Assert.StartsWith("<archmage> Failed to load atlas item: \"skill\"", err.Message);
            Assert.IsType<IOException>(err.InnerException);

            // No read outlives the load.
            Assert.Equal(0, fs.InFlight);
        }
    }
}
