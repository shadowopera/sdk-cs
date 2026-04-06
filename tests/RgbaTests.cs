using System;
using Newtonsoft.Json;
using Shadop.Archmage.Sdk;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    public class RgbaTests
    {
        // ── Parse ────────────────────────────────────────────────────────────

        [Fact]
        public void Parse_EmptyOrNull_ReturnsZero()
        {
            Assert.Equal(default(Rgba), Rgba.Parse(""));
            Assert.Equal(default(Rgba), Rgba.Parse(null));
        }

        [Fact]
        public void Parse_RgbFormat_AssumesFullAlpha()
        {
            var c = Rgba.Parse("#1A2B3C");
            Assert.Equal(0x1A, c.R);
            Assert.Equal(0x2B, c.G);
            Assert.Equal(0x3C, c.B);
            Assert.Equal(0xFF, c.A);
        }

        [Fact]
        public void Parse_RgbaFormat_AllChannels()
        {
            var c = Rgba.Parse("#1A2B3C4D");
            Assert.Equal(0x1A, c.R);
            Assert.Equal(0x2B, c.G);
            Assert.Equal(0x3C, c.B);
            Assert.Equal(0x4D, c.A);
        }

        [Fact]
        public void Parse_LowercaseHex_Accepted()
        {
            var c = Rgba.Parse("#aabbccdd");
            Assert.Equal(0xAA, c.R);
            Assert.Equal(0xBB, c.G);
            Assert.Equal(0xCC, c.B);
            Assert.Equal(0xDD, c.A);
        }

        [Theory]
        [InlineData("red")]
        [InlineData("#ZZ0000")]
        [InlineData("#12345")]    // wrong length
        [InlineData("#12345678901")] // too long
        public void Parse_InvalidString_Throws(string s)
        {
            Assert.Throws<FormatException>(() => Rgba.Parse(s));
        }

        [Fact]
        public void TryParse_InvalidString_ReturnsFalse()
        {
            Assert.False(Rgba.TryParse("notacolor", out _));
            Assert.False(Rgba.TryParse("#ZZZZZZ", out _));
        }

        // ── ToString ─────────────────────────────────────────────────────────

        [Fact]
        public void ToString_PartialAlpha_ProducesRgbaFormat()
        {
            var c = new Rgba(0x1A, 0x2B, 0x3C, 0x4D);
            Assert.Equal("#1A2B3C4D", c.ToString());
        }

        [Fact]
        public void ToString_FullAlpha_ProducesRgbFormat()
        {
            var c = new Rgba(0x1A, 0x2B, 0x3C, 0xFF);
            Assert.Equal("#1A2B3C", c.ToString());
        }

        [Fact]
        public void ToString_ZeroValue()
        {
            Assert.Equal("#00000000", default(Rgba).ToString());
        }

        // ── JSON serialization ───────────────────────────────────────────────

        [Fact]
        public void Marshal_NonZero_FullAlpha_ProducesRgbString()
        {
            var c = new Rgba(0xFF, 0x80, 0x00, 0xFF);
            var json = JsonConvert.SerializeObject(c);
            Assert.Equal("\"#FF8000\"", json);
        }

        [Fact]
        public void Marshal_NonZero_PartialAlpha_ProducesRgbaString()
        {
            var c = new Rgba(0xFF, 0x80, 0x00, 0x7F);
            var json = JsonConvert.SerializeObject(c);
            Assert.Equal("\"#FF80007F\"", json);
        }

        [Fact]
        public void Marshal_ZeroAlpha_NonZeroRgb_ProducesRgbaString()
        {
            var c = new Rgba(0xFF, 0x80, 0x00, 0x00);
            var json = JsonConvert.SerializeObject(c);
            Assert.Equal("\"#FF800000\"", json);
        }

        [Fact]
        public void Marshal_ZeroValue_ProducesEmptyString()
        {
            var json = JsonConvert.SerializeObject(default(Rgba));
            Assert.Equal("\"\"", json);
        }

        [Fact]
        public void Unmarshal_RgbaString()
        {
            var c = JsonConvert.DeserializeObject<Rgba>("\"#FF8000FF\"");
            Assert.Equal(new Rgba(0xFF, 0x80, 0x00, 0xFF), c);
        }

        [Fact]
        public void Unmarshal_RgbString_AssumesFullAlpha()
        {
            var c = JsonConvert.DeserializeObject<Rgba>("\"#FF8000\"");
            Assert.Equal(new Rgba(0xFF, 0x80, 0x00, 0xFF), c);
        }

        [Fact]
        public void Unmarshal_EmptyString_ReturnsZero()
        {
            var c = JsonConvert.DeserializeObject<Rgba>("\"\"");
            Assert.Equal(default(Rgba), c);
        }

        [Fact]
        public void Unmarshal_Null_ReturnsZero()
        {
            var c = JsonConvert.DeserializeObject<Rgba>("null");
            Assert.Equal(default(Rgba), c);
        }

        [Fact]
        public void Unmarshal_InvalidString_Throws()
        {
            Assert.Throws<JsonSerializationException>(() =>
                JsonConvert.DeserializeObject<Rgba>("\"notacolor\""));
        }

        // ── Round-trip ───────────────────────────────────────────────────────

        [Fact]
        public void RoundTrip_RandomColors()
        {
            var rnd = new Random(42);
            for (int i = 0; i < 1000; i++)
            {
                byte r = (byte)rnd.Next(256);
                byte g = (byte)rnd.Next(256);
                byte b = (byte)rnd.Next(256);
                byte a = (byte)rnd.Next(256);
                var original = new Rgba(r, g, b, a);
                var json = JsonConvert.SerializeObject(original);
                var restored = JsonConvert.DeserializeObject<Rgba>(json);
                Assert.Equal(original, restored);
            }
        }

        [Fact]
        public void RoundTrip_ZeroValue()
        {
            var original = default(Rgba);
            var json = JsonConvert.SerializeObject(original);
            var restored = JsonConvert.DeserializeObject<Rgba>(json);
            Assert.Equal(original, restored);
        }

        // ── Equality ─────────────────────────────────────────────────────────

        [Fact]
        public void Equality_SameChannels_Equal()
        {
            var a = new Rgba(1, 2, 3, 4);
            var b = new Rgba(1, 2, 3, 4);
            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
        }

        [Fact]
        public void Equality_DifferentChannels_NotEqual()
        {
            var a = new Rgba(1, 2, 3, 4);
            var b = new Rgba(1, 2, 3, 5);
            Assert.NotEqual(a, b);
            Assert.True(a != b);
        }
    }
}
