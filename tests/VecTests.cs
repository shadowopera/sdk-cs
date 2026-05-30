using System;
using Newtonsoft.Json;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public class VecTests
    {
        [Fact]
        public void TestVec2()
        {
            var rng = new Random(42);
            Func<int> genNum = () =>
            {
                var w = rng.Next(100);
                if (w < 35) return 0;
                return rng.Next();
            };

            for (int i = 0; i < 1000; i++)
            {
                var v1 = new Vec2<int>(genNum(), genNum());
                var data = JsonConvert.SerializeObject(v1);
                var v2 = JsonConvert.DeserializeObject<Vec2<int>>(data);
                Assert.Equal(v1, v2);
            }
        }

        [Fact]
        public void TestVec3()
        {
            var rng = new Random(42);
            Func<double> genNum = () =>
            {
                var w = rng.Next(100);
                if (w < 35) return 0;
                return rng.NextDouble();
            };

            for (int i = 0; i < 1000; i++)
            {
                var v1 = new Vec3<double>(genNum(), genNum(), genNum());
                var data = JsonConvert.SerializeObject(v1);
                var v2 = JsonConvert.DeserializeObject<Vec3<double>>(data);
                Assert.Equal(v1, v2);
            }
        }

        [Fact]
        public void TestVec4()
        {
            var rng = new Random(42);
            Func<uint> genNum = () =>
            {
                var w = rng.Next(100);
                if (w < 35) return 0;
                return unchecked((uint)rng.Next());
            };

            for (int i = 0; i < 1000; i++)
            {
                var v1 = new Vec4<uint>(genNum(), genNum(), genNum(), genNum());
                var data = JsonConvert.SerializeObject(v1);
                var v2 = JsonConvert.DeserializeObject<Vec4<uint>>(data);
                Assert.Equal(v1, v2);
            }
        }
    }
}
