using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Xunit;

namespace Shadop.Archmage.Sdk.Tests
{
    // The attribute names a type that TypeDescriptor cannot find, like a converter in a Godot game assembly.
    [TypeConverter("Shadop.Archmage.Sdk.Tests.UnresolvableIdTypeConverter, NoSuchAssembly")]
    struct UnresolvableId
    {
        public long Value;
    }

    class UnresolvableIdTypeConverter : ValueWrapperTypeConverter<UnresolvableId, long>
    {
        protected override UnresolvableId Create(long value) => new() { Value = value };
    }

    public class ValueWrapperTypeConverterTests
    {
        [Fact]
        public void TestRegister()
        {
            const string json = "{\"7\": 1}";
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Dictionary<UnresolvableId, int>>(json));

            new UnresolvableIdTypeConverter().Register();

            Assert.IsType<UnresolvableIdTypeConverter>(TypeDescriptor.GetConverter(typeof(UnresolvableId)));
            var dict = JsonConvert.DeserializeObject<Dictionary<UnresolvableId, int>>(json)!;
            Assert.Equal(1, dict[new UnresolvableId { Value = 7 }]);
        }
    }
}
