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

        [Fact]
        public void TestDurationMathAndProperties()
        {
            var d1 = Duration.Second * 2; // 2 seconds
            var d2 = Duration.Millisecond * 500; // 500 ms

            // Arithmetic
            Assert.Equal(new Duration(2_500_000_000), d1 + d2);
            Assert.Equal(new Duration(1_500_000_000), d1 - d2);
            Assert.Equal(new Duration(-2_000_000_000), -d1);
            Assert.Equal(new Duration(4_000_000_000), d1 * 2);
            Assert.Equal(new Duration(4_000_000_000), 2 * d1);
            Assert.Equal(new Duration(1_000_000_000), d1 / 2L);
            Assert.Equal(4, d1 / d2);
            Assert.Equal(Duration.Zero, d1 % d2);
            Assert.Equal(new Duration(500_000_000), d1 % new Duration(1_500_000_000));

            // Comparisons
            Assert.True(d1 > d2);
            Assert.True(d1 >= d2);
            Assert.True(d2 < d1);
            Assert.True(d2 <= d1);
            Assert.True(d1 != d2);
            Assert.True(d1 == new Duration(2_000_000_000));
            Assert.Equal(1, d1.CompareTo(d2));
            Assert.Equal(-1, d2.CompareTo(d1));
            Assert.Equal(0, d1.CompareTo(new Duration(2_000_000_000)));

            // Properties & Conversions
            Assert.Equal(2_000_000_000, d1.Nanoseconds());
            Assert.Equal(2_000_000, d1.Microseconds());
            Assert.Equal(2000, d1.Milliseconds());
            Assert.Equal(2.0, d1.Seconds());
            Assert.Equal(2.0 / 60.0, d1.Minutes());
            Assert.Equal(2.0 / 3600.0, d1.Hours());

            var ts = d1.ToTimeSpan();
            Assert.Equal(TimeSpan.FromSeconds(2), ts);
            Assert.Equal(d1, Duration.FromTimeSpan(ts));

            Assert.Equal(new Duration(2_000_000_000), new Duration(-2_000_000_000).Abs());

            // Truncate & Round
            var d3 = new Duration(2_400_000_000); // 2.4s
            Assert.Equal(new Duration(2_000_000_000), d3.Truncate(Duration.Second));
            Assert.Equal(new Duration(2_000_000_000), d3.Round(Duration.Second));
            
            var d4 = new Duration(2_600_000_000); // 2.6s
            Assert.Equal(new Duration(2_000_000_000), d4.Truncate(Duration.Second));
            Assert.Equal(new Duration(3_000_000_000), d4.Round(Duration.Second));
        }

        [Fact]
        public void TestDurationToString()
        {
            Assert.Equal("0s", Duration.Zero.ToString());
            Assert.Equal("1ns", Duration.Nanosecond.ToString());
            Assert.Equal("1us", Duration.Microsecond.ToString());
            Assert.Equal("1ms", Duration.Millisecond.ToString());
            Assert.Equal("1s", Duration.Second.ToString());
            Assert.Equal("1m", Duration.Minute.ToString());
            Assert.Equal("1h", Duration.Hour.ToString());

            Assert.Equal("1h30m5s", (Duration.Hour + Duration.Minute * 30 + Duration.Second * 5).ToString());
            Assert.Equal("-1h30m5s", (-(Duration.Hour + Duration.Minute * 30 + Duration.Second * 5)).ToString());
            Assert.Equal("500ms", (Duration.Millisecond * 500).ToString());
            Assert.Equal("1s200ms", (Duration.Second + Duration.Millisecond * 200).ToString());
        }
    }
}
