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
            public string subject { get; set; } = "";
            public long[]? input { get; set; }
            public Duration expected { get; set; }
            public string expErr { get; set; } = "";
        }

        [Fact]
        public void TestParseDurationShards()
        {
            var dataset = new DurationTrial[]
            {
                new DurationTrial {
                    subject = "nil input", input = null, expected = Duration.Zero, expErr = "" },
                new DurationTrial {
                    subject = "empty slice", input = Array.Empty<long>(), expected = Duration.Zero, expErr = "" },
                new DurationTrial {
                    subject = "seconds", input = new long[] { 0, 5 }, expected = new Duration(5_000_000_000), expErr = "" },
                new DurationTrial {
                    subject = "milliseconds", input = new long[] { 1, 500 }, expected = new Duration(500_000_000), expErr = "" },
                new DurationTrial {
                    subject = "microseconds", input = new long[] { 2, 750 }, expected = new Duration(750_000), expErr = "" },
                new DurationTrial {
                    subject = "nanoseconds", input = new long[] { 3, 250 }, expected = new Duration(250), expErr = "" },
                new DurationTrial {
                    subject = "seconds with nanoseconds", input = new long[] { 4, 3, 500000001 }, expected = new Duration(3_500_000_001), expErr = "" },
                new DurationTrial {
                    subject = "invalid format 2", input = new long[] { 5, 100 }, expected = Duration.Zero, expErr = "invalid duration shard type" },
                new DurationTrial {
                    subject = "invalid format 3", input = new long[] { 0, 5, 100 }, expected = new Duration(5_000_000_000), expErr = "" },
                new DurationTrial {
                    subject = "unsupported length", input = new long[] { 4, 3, 500000000, 0 }, expected = Duration.Zero, expErr = "mixed duration shards must have 3 elements" },
                new DurationTrial {
                    subject = "slice with seconds format", input = new long[] { 0, 10 }, expected = new Duration(10_000_000_000), expErr = "" },
                new DurationTrial {
                    subject = "slice with seconds and nanoseconds", input = new long[] { 4, 2, 750000001 }, expected = new Duration(2_750_000_001), expErr = "" },
            };

            foreach (var tt in dataset)
            {
                if (!string.IsNullOrEmpty(tt.expErr))
                {
                    var ex = Assert.Throws<ArchmageException>(() => Archmage.ParseDurationShards(tt.input));
                    Assert.Contains(tt.expErr, ex.Message);
                }
                else
                {
                    var r = Archmage.ParseDurationShards(tt.input);
                    Assert.Equal(tt.expected, r);

                    var d1 = tt.expected;
                    var data = JsonConvert.SerializeObject(d1);
                    var d2 = JsonConvert.DeserializeObject<Duration>(data);
                    Assert.Equal(tt.expected, d2);

                    var inputData = JsonConvert.SerializeObject(tt.input);
                    var d3 = JsonConvert.DeserializeObject<Duration>(inputData);
                    Assert.Equal(tt.expected, d3);

                    if (tt.subject != "empty slice" && !tt.subject.StartsWith("slice with") && !tt.subject.StartsWith("invalid format") && !tt.subject.StartsWith("unsupported length"))
                    {
                        var shards = Archmage.ShardDuration(r);
                        Assert.Equal(tt.input, shards);
                    }
                }
            }
        }
    }
}
