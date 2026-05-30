using System;
using Newtonsoft.Json;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public class MinMaxTests
    {
        [Fact]
        public void TestJsonRoundTrip()
        {
            var a = new MinMax<int>(-7, 42);
            var b = JsonConvert.DeserializeObject<MinMax<int>>(JsonConvert.SerializeObject(a));
            Assert.Equal(a, b);

            var c = new MinMax<double>(1.5, 9.25);
            var d = JsonConvert.DeserializeObject<MinMax<double>>(JsonConvert.SerializeObject(c));
            Assert.Equal(c, d);
        }

        [Fact]
        public void TestJsonPropertyNames()
        {
            var json = JsonConvert.SerializeObject(new MinMax<int>(3, 8));
            Assert.Equal("{\"min\":3,\"max\":8}", json);

            var mm = JsonConvert.DeserializeObject<MinMax<int>>("{\"min\":10,\"max\":20}");
            Assert.Equal(10, mm.Min);
            Assert.Equal(20, mm.Max);
        }

        [Fact]
        public void TestEquality()
        {
            Assert.True(new MinMax<int>(1, 2) == new MinMax<int>(1, 2));
            Assert.True(new MinMax<int>(1, 2) != new MinMax<int>(1, 3));
            Assert.Equal(new MinMax<float>(0f, 1f).GetHashCode(), new MinMax<float>(0f, 1f).GetHashCode());
        }

        [Fact]
        public void TestIntegerSampleInBounds()
        {
            var rng = new Random(42);
            var ranges = new[]
            {
                new MinMax<int>(0, 10),
                new MinMax<int>(-10, 10),
                new MinMax<int>(-500_000_000, 500_000_000),
                new MinMax<int>(int.MaxValue - 1000, int.MaxValue),
                new MinMax<int>(5, 5),
            };

            foreach (var mm in ranges)
            {
                for (int i = 0; i < 10000; i++)
                {
                    var v = mm.Sample(rng);
                    Assert.True(v >= mm.Min && v <= mm.Max, $"{v} not in [{mm.Min}, {mm.Max}]");
                }
            }
        }

        [Fact]
        public void TestUnsignedAndWideSampleInBounds()
        {
            var rng = new Random(42);

            var b = new MinMax<byte>(10, 250);
            var s = new MinMax<sbyte>(-100, 100);
            var u = new MinMax<uint>(uint.MaxValue - 1_000_000_000, uint.MaxValue);
            var l = new MinMax<long>(long.MaxValue - 1_000_000_000, long.MaxValue);
            var ul = new MinMax<ulong>(ulong.MaxValue - 1_000_000_000, ulong.MaxValue);

            for (int i = 0; i < 10000; i++)
            {
                var vb = b.Sample(rng);
                Assert.True(vb >= b.Min && vb <= b.Max);

                var vs = s.Sample(rng);
                Assert.True(vs >= s.Min && vs <= s.Max);

                var vu = u.Sample(rng);
                Assert.True(vu >= u.Min && vu <= u.Max);

                var vl = l.Sample(rng);
                Assert.True(vl >= l.Min && vl <= l.Max);

                var vul = ul.Sample(rng);
                Assert.True(vul >= ul.Min && vul <= ul.Max);
            }
        }

        [Fact]
        public void TestFloatSampleInBounds()
        {
            var rng = new Random(42);
            var f = new MinMax<float>(-2.5f, 7.5f);
            var d = new MinMax<double>(100.0, 200.0);

            for (int i = 0; i < 10000; i++)
            {
                var vf = f.Sample(rng);
                Assert.True(vf >= f.Min && vf <= f.Max, $"{vf} not in [{f.Min}, {f.Max}]");

                var vd = d.Sample(rng);
                Assert.True(vd >= d.Min && vd <= d.Max, $"{vd} not in [{d.Min}, {d.Max}]");
            }
        }

        [Fact]
        public void TestIntegerEndpointsReachable()
        {
            var rng = new Random(42);
            var mm = new MinMax<int>(0, 5);
            bool hitMin = false, hitMax = false;

            for (int i = 0; i < 10000 && !(hitMin && hitMax); i++)
            {
                var v = mm.Sample(rng);
                if (v == mm.Min) hitMin = true;
                if (v == mm.Max) hitMax = true;
            }

            Assert.True(hitMin, "Min was never sampled");
            Assert.True(hitMax, "Max was never sampled");
        }

        [Fact]
        public void TestDegenerateRange()
        {
            var rng = new Random(42);
            var mm = new MinMax<long>(123, 123);
            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(123, mm.Sample(rng));
            }
        }
    }
}
