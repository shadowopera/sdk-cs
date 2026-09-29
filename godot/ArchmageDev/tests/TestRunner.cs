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
        /// <summary>
        /// When true, the test runs only when --filter matches it; without --filter, it is skipped.
        /// </summary>
        public bool Explicit { get; set; }
    }

    /// <summary>
    /// Runs every [GodotTest] method one by one on the Godot main thread, writes a summary and quits.
    /// </summary>
    /// <remarks>
    /// From the project directory, pass it as the scene to run: <c>godot --path godot/ArchmageDev
    /// res://tests/test_runner.tscn -- [--filter &lt;regex&gt;] [--summary &lt;file&gt;]</c>. The release template does
    /// not accept a scene path, so the export preset "macOS (tests)" makes it the main scene; pass only the arguments
    /// after <c>--</c>. The filter is matched against <c>Class.Method</c>. The exit code is 0 when all tests pass, and
    /// 1 otherwise.
    /// </remarks>
    public partial class TestRunner : Node
    {
        static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// The managed thread ID of the Godot main thread.
        /// </summary>
        public static int MainThreadId { get; private set; }

        enum Outcome
        {
            Passed,
            Failed,
            Skipped,
        }

        record Result(string Name, Outcome Outcome, TimeSpan Duration, string? Message);

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
            foreach (var (name, method, isExplicit) in tests)
            {
                if (isExplicit && filter is null)
                {
                    results.Add(new Result(name, Outcome.Skipped, TimeSpan.Zero, null));
                    continue;
                }
                GD.Print($"[TestRunner] {name}");
                results.Add(await RunTest(name, method));
            }

            var summary = Summarize(results);
            GD.Print(summary);
            if (summaryFile is not null)
                File.WriteAllText(summaryFile, summary);

            var run = results.Where(r => r.Outcome != Outcome.Skipped).ToList();
            return run.Count > 0 && run.All(r => r.Outcome == Outcome.Passed) ? 0 : 1;
        }

        static IEnumerable<(string Name, MethodInfo Method, bool Explicit)> Discover()
        {
            return typeof(TestRunner).Assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                .Select(m => (Method: m, Attr: m.GetCustomAttribute<GodotTestAttribute>()))
                .Where(t => t.Attr is not null)
                .Select(t => ($"{t.Method.DeclaringType!.Name}.{t.Method.Name}", t.Method, t.Attr!.Explicit))
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
                    return new Result(name, Outcome.Failed, stopwatch.Elapsed, $"Timed out after {Timeout.TotalSeconds}s.");
                await task;
                return new Result(name, Outcome.Passed, stopwatch.Elapsed, null);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException { InnerException: { } inner })
                    ex = inner;
                var message = ex is AssertionException ? ex.Message : ex.ToString();
                return new Result(name, Outcome.Failed, stopwatch.Elapsed, message);
            }
        }

        // The same format as scripts/unity-test-summary.sh.
        static string Summarize(List<Result> results)
        {
            var mode = OS.HasFeature("template") ? "Exported (.pck)" : "Project directory (--path)";
            var passed = results.Count(r => r.Outcome == Outcome.Passed);
            var failed = results.Count(r => r.Outcome == Outcome.Failed);
            var skipped = results.Count(r => r.Outcome == Outcome.Skipped);

            var sb = new StringBuilder();
            sb.AppendLine($"Mode:  {mode}");
            sb.AppendLine($"Tests: total={results.Count} passed={passed} failed={failed} skipped={skipped}");
            sb.AppendLine();

            sb.AppendLine("Test results (duration is the wall time of each test, not a cost measurement)");
            var rows = new List<string[]> { new[] { "TEST", "RESULT", "DURATION" } };
            rows.AddRange(results.Select(r => new[]
            {
                r.Name, r.Outcome.ToString(), $"{r.Duration.TotalSeconds:0.00}s"
            }));
            AppendTable(sb, rows);

            var failures = results.Where(r => r.Outcome == Outcome.Failed).ToList();
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

            sb.AppendLine();
            sb.AppendLine("FileAccess load cost (from FileAccessCostTests, frames stretched to ~16 ms)");
            if (FileAccessCostTests.Measurements.Count == 0)
            {
                sb.AppendLine("  not measured (run with --filter FileAccessCostTests)");
            }
            else
            {
                var costRows = new List<string[]> { new[] { "LOADING", "FRAMES", "TIME" } };
                costRows.AddRange(FileAccessCostTests.Measurements.Select(m => new[]
                {
                    m.Loading, m.Frames.ToString(), $"{m.Elapsed.TotalMilliseconds:0.0}ms"
                }));
                AppendTable(sb, costRows);
            }
            return sb.ToString();
        }

        static void AppendTable(StringBuilder sb, List<string[]> rows)
        {
            var columns = rows[0].Length;
            var widths = Enumerable.Range(0, columns).Select(i => rows.Max(row => row[i].Length)).ToArray();
            foreach (var row in rows)
            {
                var cells = row.Select((cell, i) => i < columns - 1 ? cell.PadRight(widths[i]) : cell);
                sb.AppendLine($"  {string.Join("  ", cells)}");
            }
        }
    }
}
