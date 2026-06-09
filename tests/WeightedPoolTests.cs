using System;
using Newtonsoft.Json;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public class WeightedPoolTests
    {
        [Fact]
        public void TestEmptyThrows()
        {
            var rng = new Random(1);
            var wp = new WeightedPool<int>();
            Assert.Throws<ArchmageException>(() => wp.Sample(rng));
        }

        [Fact]
        public void TestZeroTotalThrows()
        {
            var rng = new Random(1);
            var wp = new WeightedPool<int>(new[] { 1, 2, 3 }, new[] { 0, 0, 0 });
            Assert.Throws<ArchmageException>(() => wp.SampleIndex(rng));
        }

        [Fact]
        public void TestSingleElement()
        {
            var rng = new Random(1);
            var wp = new WeightedPool<string>(new[] { "only" }, new[] { 7 });
            for (int i = 0; i < 100; i++)
            {
                Assert.Equal("only", wp.Sample(rng));
                Assert.Equal(0, wp.SampleIndex(rng));
            }
        }

        [Fact]
        public void TestZeroWeightNeverSelected()
        {
            var rng = new Random(1);
            var wp = new WeightedPool<int>(new[] { 10, 20, 30 }, new[] { 5, 0, 5 });
            for (int i = 0; i < 10000; i++)
            {
                Assert.NotEqual(1, wp.SampleIndex(rng));
            }
        }

        [Fact]
        public void TestDistribution()
        {
            var rng = new Random(1);
            var wp = new WeightedPool<int>(new[] { 0, 1, 2, 3 }, new[] { 1, 2, 3, 4 });

            long total = 0;
            foreach (int w in wp.Weights!)
            {
                total += w;
            }

            const int n = 1_000_000;
            var counts = new int[wp.Count];
            for (int i = 0; i < n; i++)
            {
                counts[wp.SampleIndex(rng)]++;
            }

            for (int i = 0; i < wp.Weights!.Length; i++)
            {
                double want = (double)wp.Weights[i] / total;
                double got = (double)counts[i] / n;
                Assert.True(Math.Abs(got - want) <= 0.005, $"index {i}: want ~{want:F4}, got {got:F4}");
            }
        }

        [Fact]
        public void TestLargeWeightsDistribution()
        {
            // Large equal weights (summing to exactly 1,000,000,000) must stay within the limit
            // and select all indices with roughly equal probability.
            var rng = new Random(1);
            var wp = new WeightedPool<int>(
                new[] { 0, 1, 2 },
                new[] { 333_333_333, 333_333_333, 333_333_334 });

            var seen = new bool[wp.Count];
            for (int i = 0; i < 10000; i++)
            {
                int idx = wp.SampleIndex(rng);
                Assert.InRange(idx, 0, wp.Count - 1);
                seen[idx] = true;
            }
            for (int i = 0; i < seen.Length; i++)
            {
                Assert.True(seen[i], $"index {i} never selected despite equal weights");
            }
        }

        [Fact]
        public void TestTotalOverLimitThrows()
        {
            var rng = new Random(1);
            var wp = new WeightedPool<int>(new[] { 0, 1 }, new[] { 500_000_001, 500_000_000 });
            Assert.Throws<ArchmageException>(() => wp.SampleIndex(rng));
        }

        [Fact]
        public void TestJsonRoundTrip()
        {
            var a = new WeightedPool<int>(new[] { 1, 2, 3 }, new[] { 4, 5, 6 });
            var b = JsonConvert.DeserializeObject<WeightedPool<int>>(JsonConvert.SerializeObject(a))!;
            Assert.Equal(a.Items, b.Items);
            Assert.Equal(a.Weights, b.Weights);
        }

        [Fact]
        public void TestJsonPropertyNames()
        {
            var json = JsonConvert.SerializeObject(new WeightedPool<int>(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }));
            Assert.Equal("{\"items\":[1,2,3],\"weights\":[4,5,6]}", json);
        }
    }
}
