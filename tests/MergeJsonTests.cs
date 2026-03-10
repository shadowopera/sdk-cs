using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Shadop.Archmage;
using Xunit;

namespace Shadop.Archmage.Tests
{
    public class MergeJsonTests
    {
        private static void CallMergeJson(object target, string json)
        {
            var method = typeof(Archmage).GetMethod("MergeJson", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(null, new object[] { target, json, null! });
        }

        public class NestedObject
        {
            [JsonProperty("foo")]
            public FooObject? Foo { get; set; }

            [JsonProperty("xxx")]
            public string Xxx { get; set; } = "";
        }

        public class FooObject
        {
            [JsonProperty("bar")]
            public List<int>? Bar { get; set; }

            [JsonProperty("value")]
            public int Value { get; set; }
        }

        [Fact]
        public void TestMergeJson_NestedObjectAndArrayReplacement()
        {
            var target = new NestedObject
            {
                Xxx = "Base",
                Foo = new FooObject
                {
                    Value = 10,
                    Bar = new List<int> { 1, 2, 3 }
                }
            };

            // Test 1: Merge partial properties only (deep merge)
            var patch1 = "{ \"foo\": { \"value\": 42 } }";
            CallMergeJson(target, patch1);

            Assert.Equal("Base", target.Xxx);
            Assert.NotNull(target.Foo);
            Assert.Equal(42, target.Foo.Value);
            Assert.Equal(new List<int> { 1, 2, 3 }, target.Foo.Bar); // List remains unaffected

            // Test 2: Nested Array should be completely replaced, not merged
            var patch2 = "{ \"foo\": { \"bar\": [ 9, 8 ] } }";
            CallMergeJson(target, patch2);

            Assert.NotNull(target.Foo);
            Assert.Equal(42, target.Foo.Value);
            Assert.Equal(new List<int> { 9, 8 }, target.Foo.Bar); // The old [1, 2, 3] is completely replaced by [9, 8]

            // Test 3: Top-level property override and null field replacement
            var patch3 = "{ \"xxx\": \"Updated\", \"foo\": null }";
            CallMergeJson(target, patch3);

            Assert.Equal("Updated", target.Xxx);
            Assert.Null(target.Foo);
        }

        [Fact]
        public void TestMergeJson_TopLevelArrayReplacement()
        {
            var target = new List<int> { 1, 2, 3 };
            var patch = "[4, 5]";

            CallMergeJson(target, patch);

            // The entire list should be cleared and replaced
            Assert.Equal(new List<int> { 4, 5 }, target);
        }
    }
}
