using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Conf;
using Shadop.Archmage.Sdk;

namespace ArchmageDev.Tests
{
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

    /// <summary>
    /// A logger and a progress receiver that record the events and the threads they are called on.
    /// </summary>
    sealed class ThreadProbeLog : IAtlasLogger, IProgress<AtlasLoadEvent>
    {
        public ConcurrentBag<int> LoggerThreadIds { get; } = new();
        public ConcurrentBag<int> ProgressThreadIds { get; } = new();
        public ConcurrentBag<AtlasLoadEvent> Events { get; } = new();

        public void Info(string message) => LoggerThreadIds.Add(Environment.CurrentManagedThreadId);

        public void Report(AtlasLoadEvent value)
        {
            ProgressThreadIds.Add(Environment.CurrentManagedThreadId);
            Events.Add(value);
        }

        public int CountEvents(string key, AtlasLoadStage stage) => Events.Count(e => e.Key == key && e.Stage == stage);
    }
}
