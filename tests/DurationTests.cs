using System;
using Newtonsoft.Json;
using Shadop.Archmage;
using Xunit;

namespace Shadop.Archmage.Tests
{
    public class DurationTests
    {
        class DurationTrial
        {
            public string Subject { get; set; } = "";
            public long[]? Input { get; set; }
            public Duration Expected { get; set; }
            public string ExpErr { get; set; } = "";
        }

        [Fact]
        public void TestParseDurationShards()
        {
            var dataset = new DurationTrial[]
            {
                new() { Subject = "nil input", Input = null, Expected = Duration.Zero, ExpErr = "" },
                new() { Subject = "empty slice", Input = Array.Empty<long>(), Expected = Duration.Zero, ExpErr = "" },
                new() { Subject = "seconds", Input = new long[] { 0, 5 }, Expected = new Duration(5_000_000_000), ExpErr = "" },
                new() { Subject = "milliseconds", Input = new long[] { 1, 500 }, Expected = new Duration(500_000_000), ExpErr = "" },
                new() { Subject = "microseconds", Input = new long[] { 2, 750 }, Expected = new Duration(750_000), ExpErr = "" },
                new() { Subject = "nanoseconds", Input = new long[] { 3, 250 }, Expected = new Duration(250), ExpErr = "" },
                new() { Subject = "seconds with nanoseconds", Input = new long[] { 4, 3, 500000001 }, Expected = new Duration(3_500_000_001), ExpErr = "" },
                new() { Subject = "invalid format 2", Input = new long[] { 5, 100 }, Expected = Duration.Zero, ExpErr = "invalid duration shard type" },
                new() { Subject = "invalid format 3", Input = new long[] { 0, 5, 100 }, Expected = new Duration(5_000_000_000), ExpErr = "" },
                new() { Subject = "unsupported length", Input = new long[] { 4, 3, 500000000, 0 }, Expected = Duration.Zero, ExpErr = "mixed duration shards must have 3 elements" },
                new() { Subject = "slice with seconds format", Input = new long[] { 0, 10 }, Expected = new Duration(10_000_000_000), ExpErr = "" },
                new() { Subject = "slice with seconds and nanoseconds", Input = new long[] { 4, 2, 750000001 }, Expected = new Duration(2_750_000_001), ExpErr = "" },
            };

            foreach (var tt in dataset)
            {
                if (!string.IsNullOrEmpty(tt.ExpErr))
                {
                    var ex = Assert.Throws<ArchmageException>(() => Archmage.ParseDurationShards(tt.Input));
                    Assert.Contains(tt.ExpErr, ex.Message);
                }
                else
                {
                    var r = Archmage.ParseDurationShards(tt.Input);
                    Assert.Equal(tt.Expected, r);

                    var d1 = tt.Expected;
                    var data = JsonConvert.SerializeObject(d1);
                    var d2 = JsonConvert.DeserializeObject<Duration>(data);
                    Assert.Equal(tt.Expected, d2);

                    var inputData = JsonConvert.SerializeObject(tt.Input);
                    var d3 = JsonConvert.DeserializeObject<Duration>(inputData);
                    Assert.Equal(tt.Expected, d3);

                    if (tt.Subject != "empty slice" && !tt.Subject.StartsWith("slice with") && !tt.Subject.StartsWith("invalid format") && !tt.Subject.StartsWith("unsupported length"))
                    {
                        var shards = Archmage.ShardDuration(r);
                        Assert.Equal(tt.Input, shards);
                    }
                }
            }
        }
    }
}
