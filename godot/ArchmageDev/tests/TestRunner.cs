using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Conf;
using Godot;
using Environment = System.Environment;

namespace ArchmageDev.Tests
{
    /// <summary>
    /// Marks a test method. The method must be static, take no parameters and return <see cref="Task"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class GodotTestAttribute : Attribute
    {
    }

    /// <summary>
    /// Runs every [GodotTest] method one by one on the Godot main thread, writes a summary and quits.
    /// </summary>
    /// <remarks>
    /// Start it as the scene of the run: <c>godot --path godot/ArchmageDev res://tests/test_runner.tscn -- [--filter
    /// &lt;regex&gt;] [--summary &lt;file&gt;]</c>. The filter is matched against <c>Class.Method</c>. The exit code is
    /// 0 when all tests pass, and 1 otherwise.
    /// </remarks>
    public partial class TestRunner : Node
    {
        static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// The managed thread ID of the Godot main thread.
        /// </summary>
        public static int MainThreadId { get; private set; }

        record Result(string Name, bool Passed, TimeSpan Duration, string? Message);

        public override async void _Ready()
        {
            var exitCode = 1;
            try
            {
                exitCode = await RunAsync();
            }
            catch (Exception ex)
            {
                GD.PrintErr($"[TestRunner] {ex}");
            }
            GetTree().Quit(exitCode);
        }

        static async Task<int> RunAsync()
        {
            MainThreadId = Environment.CurrentManagedThreadId;
            if (OS.GetThreadCallerId() != OS.GetMainThreadId())
                throw new InvalidOperationException("The runner is not on the Godot main thread.");
            if (SynchronizationContext.Current is not GodotSynchronizationContext)
                throw new InvalidOperationException("The main thread has no GodotSynchronizationContext.");

            string? filter = null;
            string? summaryFile = null;
            var args = OS.GetCmdlineUserArgs();
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--filter" when i + 1 < args.Length:
                        filter = args[++i];
                        break;
                    case "--summary" when i + 1 < args.Length:
                        summaryFile = args[++i];
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {args[i]}");
                }
            }

            var tests = Discover()
                .Where(t => filter is null || Regex.IsMatch(t.Name, filter))
                .ToList();

            var results = new List<Result>();
            foreach (var (name, method) in tests)
            {
                GD.Print($"[TestRunner] {name}");
                results.Add(await RunTest(name, method));
            }

            var summary = Summarize(results);
            GD.Print(summary);
            if (summaryFile is not null)
                File.WriteAllText(summaryFile, summary);

            return results.Count > 0 && results.All(r => r.Passed) ? 0 : 1;
        }

        static IEnumerable<(string Name, MethodInfo Method)> Discover()
        {
            return typeof(TestRunner).Assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Where(m => m.GetCustomAttribute<GodotTestAttribute>() is not null)
                .Select(m => ($"{m.DeclaringType!.Name}.{m.Name}", m))
                .OrderBy(t => t.Item1, StringComparer.Ordinal);
        }

        static async Task<Result> RunTest(string name, MethodInfo method)
        {
            ConfigAtlas.Instance = null!;
            L10n.GetI18n = null!;
            L10n.GetPreferredLanguage = null!;

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var task = (Task)method.Invoke(null, null)!;
                if (await Task.WhenAny(task, Task.Delay(Timeout)) != task)
                    return new Result(name, false, stopwatch.Elapsed, $"Timed out after {Timeout.TotalSeconds}s.");
                await task;
                return new Result(name, true, stopwatch.Elapsed, null);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException { InnerException: { } inner })
                    ex = inner;
                var message = ex is AssertionException ? ex.Message : ex.ToString();
                return new Result(name, false, stopwatch.Elapsed, message);
            }
        }

        // The same format as scripts/unity-test-summary.sh.
        static string Summarize(List<Result> results)
        {
            var mode = OS.HasFeature("template") ? "Exported (.pck)" : "Project directory (--path)";
            var passed = results.Count(r => r.Passed);

            var sb = new StringBuilder();
            sb.AppendLine($"Mode:  {mode}");
            sb.AppendLine($"Tests: total={results.Count} passed={passed} failed={results.Count - passed} skipped=0");
            sb.AppendLine();

            sb.AppendLine("Test results (duration is the wall time of each test)");
            var rows = new List<string[]> { new[] { "TEST", "RESULT", "DURATION" } };
            rows.AddRange(results.Select(r => new[]
            {
                r.Name, r.Passed ? "Passed" : "Failed", $"{r.Duration.TotalSeconds:0.00}s"
            }));
            var widths = Enumerable.Range(0, 3).Select(i => rows.Max(row => row[i].Length)).ToArray();
            foreach (var row in rows)
                sb.AppendLine($"  {row[0].PadRight(widths[0])}  {row[1].PadRight(widths[1])}  {row[2]}");

            var failures = results.Where(r => !r.Passed).ToList();
            if (failures.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Failures");
                foreach (var r in failures)
                {
                    sb.AppendLine($"  - {r.Name}");
                    foreach (var line in r.Message!.Split('\n'))
                        sb.AppendLine($"      {line.TrimEnd('\r')}");
                }
            }
            return sb.ToString();
        }
    }
}
