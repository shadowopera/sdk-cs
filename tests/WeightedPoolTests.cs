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
        public void TestNoiseEmptyThrows()
        {
            var wp = new WeightedPool<int>();
            Assert.Throws<ArchmageException>(() => wp.SampleWithNoise(0.5f));
        }

        [Fact]
        public void TestNoiseZeroTotalThrows()
        {
            var wp = new WeightedPool<int>(new[] { 1, 2, 3 }, new[] { 0, 0, 0 });
            Assert.Throws<ArchmageException>(() => wp.SampleIndexWithNoise(0.5f));
        }

        [Fact]
        public void TestNoiseTotalOverLimitThrows()
        {
            var wp = new WeightedPool<int>(new[] { 0, 1 }, new[] { 500_000_001, 500_000_000 });
            Assert.Throws<ArchmageException>(() => wp.SampleIndexWithNoise(0.5f));
        }

        [Fact]
        public void TestNoiseBoundaries()
        {
            // Leading and trailing zero-weight items must be skipped at the boundaries.
            var wp = new WeightedPool<int>(new[] { 0, 1, 2, 3 }, new[] { 0, 5, 5, 0 });
            Assert.Equal(1, wp.SampleIndexWithNoise(-0.1f)); // below 0 -> first non-zero index
            Assert.Equal(1, wp.SampleIndexWithNoise(0f));
            Assert.Equal(2, wp.SampleIndexWithNoise(1f));    // >= 1 -> last non-zero index
            Assert.Equal(2, wp.SampleIndexWithNoise(2f));
        }

        [Fact]
        public void TestNoiseProportional()
        {
            // value = floor(noise * total); index i owns [cumBefore, cumBefore + weight).
            // Noise picked inside each band to avoid float rounding on bucket boundaries.
            var wp = new WeightedPool<int>(new[] { 0, 1, 2 }, new[] { 1, 2, 3 }); // total 6
            Assert.Equal(0, wp.SampleIndexWithNoise(0f));     // value 0   -> [0,1)
            Assert.Equal(0, wp.SampleIndexWithNoise(0.1f));   // value 0   -> [0,1)
            Assert.Equal(1, wp.SampleIndexWithNoise(0.25f));  // value 1   -> [1,3)
            Assert.Equal(1, wp.SampleIndexWithNoise(0.4f));   // value 2   -> [1,3)
            Assert.Equal(2, wp.SampleIndexWithNoise(0.6f));   // value 3   -> [3,6)
            Assert.Equal(2, wp.SampleIndexWithNoise(0.9f));   // value 5   -> [3,6)
        }

        [Fact]
        public void TestNoiseZeroWeightNeverSelected()
        {
            var wp = new WeightedPool<int>(new[] { 10, 20, 30 }, new[] { 5, 0, 5 });
            for (int i = 0; i <= 1000; i++)
            {
                Assert.NotEqual(1, wp.SampleIndexWithNoise(i / 1000f));
            }
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
