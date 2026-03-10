using System;
using Newtonsoft.Json;
using Shadop.Archmage;
using Xunit;

namespace Shadop.Archmage.Tests
{
    public class RefTests
    {
        [Fact]
        public void TestXRef()
        {
            string str = "foo";
            int[] dataset = { 0, -1, 1, 42, 1000000 };

            foreach (var v in dataset)
            {
                var ref1 = new XRef<int, string>(v, str);
                var data = JsonConvert.SerializeObject(ref1);

                if (!int.TryParse(data, out _))
                {
                    Assert.Fail($"expected marshaled data to be integer, got {data}");
                }

                var ref2 = JsonConvert.DeserializeObject<XRef<int, string>>(data);
                Assert.Equal(v, ref2.RawValue);
                Assert.Null(ref2.Ref);
            }
        }
    }
}
