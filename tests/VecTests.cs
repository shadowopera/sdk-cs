using System;
using Newtonsoft.Json;
using Shadop.Archmage;
using Xunit;

namespace Shadop.Archmage.Tests
{
    public class VecTests
    {
        [Fact]
        public void TestVec2()
        {
            var rnd = new Random(42);
            Func<int> genNum = () =>
            {
                var w = rnd.Next(100);
                if (w < 35) return 0;
                return rnd.Next();
            };

            for (int i = 0; i < 1000; i++)
            {
                var v1 = new Vec2<int>(genNum(), genNum());
                var data = JsonConvert.SerializeObject(v1);
                var v2 = JsonConvert.DeserializeObject<Vec2<int>>(data);
                Assert.Equal(v1, v2);
            }

            var vec = new Vec2<int>(1, 2);
            vec = JsonConvert.DeserializeObject<Vec2<int>>("null");
            Assert.Equal(new Vec2<int>(0, 0), vec);
        }

        [Fact]
        public void TestVec3()
        {
            var rnd = new Random(42);
            Func<double> genNum = () =>
            {
                var w = rnd.Next(100);
                if (w < 35) return 0;
                return rnd.NextDouble();
            };

            for (int i = 0; i < 1000; i++)
            {
                var v1 = new Vec3<double>(genNum(), genNum(), genNum());
                var data = JsonConvert.SerializeObject(v1);
                var v2 = JsonConvert.DeserializeObject<Vec3<double>>(data);
                Assert.Equal(v1, v2);
            }

            var vec = new Vec3<double>(1, 2, 3);
            vec = JsonConvert.DeserializeObject<Vec3<double>>("null");
            Assert.Equal(new Vec3<double>(0, 0, 0), vec);
        }

        [Fact]
        public void TestVec4()
        {
            var rnd = new Random(42);
            Func<uint> genNum = () =>
            {
                var w = rnd.Next(100);
                if (w < 35) return 0;
                return unchecked((uint)rnd.Next());
            };

            for (int i = 0; i < 1000; i++)
            {
                var v1 = new Vec4<uint>(genNum(), genNum(), genNum(), genNum());
                var data = JsonConvert.SerializeObject(v1);
                var v2 = JsonConvert.DeserializeObject<Vec4<uint>>(data);
                Assert.Equal(v1, v2);
            }

            var vec = new Vec4<uint>(1, 2, 3, 4);
            vec = JsonConvert.DeserializeObject<Vec4<uint>>("null");
            Assert.Equal(new Vec4<uint>(0, 0, 0, 0), vec);
        }
    }
}
