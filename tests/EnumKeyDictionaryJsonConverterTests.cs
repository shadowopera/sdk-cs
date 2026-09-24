using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public class EnumKeyDictionaryJsonConverterTests
    {
        enum Color : byte
        {
            Red = 1,
            Blue = 10,
        }

        [Flags]
        enum Mask : ushort
        {
            A = 1,
            B = 2,
        }

        static readonly JsonSerializerSettings _settings = new()
        {
            Converters = { new EnumKeyDictionaryJsonConverter() },
        };

        [Fact]
        public void TestWriteJson_NumericKeys()
        {
            var dict = new Dictionary<Color, string> { { Color.Red, "a" }, { Color.Blue, "b" } };
            Assert.Equal("{\"1\":\"a\",\"10\":\"b\"}", JsonConvert.SerializeObject(dict, _settings));
        }

        [Fact]
        public void TestWriteJson_CombinedFlags()
        {
            var dict = new Dictionary<Mask, int> { { Mask.A | Mask.B, 7 } };
            Assert.Equal("{\"3\":7}", JsonConvert.SerializeObject(dict, _settings));
        }

        [Fact]
        public void TestRoundTrip()
        {
            var dict = new Dictionary<Color, List<int>> { { Color.Blue, new List<int> { 1, 2 } } };
            var json = JsonConvert.SerializeObject(dict, _settings);
            var back = JsonConvert.DeserializeObject<Dictionary<Color, List<int>>>(json, _settings)!;
            Assert.Equal(new List<int> { 1, 2 }, back[Color.Blue]);
        }

        [Fact]
        public void TestCanConvert()
        {
            var converter = new EnumKeyDictionaryJsonConverter();
            Assert.True(converter.CanConvert(typeof(Dictionary<Color, string>)));
            Assert.False(converter.CanConvert(typeof(Dictionary<int, string>)));
            Assert.False(converter.CanConvert(typeof(List<Color>)));
        }
    }
}
