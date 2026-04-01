#nullable enable

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Newtonsoft.Json converter for XRef types.
    /// </summary>
    /// <remarks>
    /// <para>This converter only serializes/deserializes the CfgId property.
    /// The Ref property is never serialized to JSON and is ignored during deserialization.
    /// The Ref property should be populated during the reference binding phase (via Atlas.BindRefs).</para>
    /// </remarks>
    public class XRefJsonConverter : JsonConverter
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo> _propCache = new();

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(XRef<,>);
        }

        /// <summary>
        /// Reads JSON value into CfgId; Ref set to null (bound later via BindRefs).
        /// </summary>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return Activator.CreateInstance(objectType);

            Type keyType = objectType.GetGenericArguments()[0];
            var cfgId = serializer.Deserialize(reader, keyType)!;
            return Activator.CreateInstance(objectType, cfgId, null);
        }

        /// <summary>
        /// Serializes XRef as CfgId only (Ref property ignored).
        /// </summary>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var prop = _propCache.GetOrAdd(value!.GetType(), t => t.GetProperty("CfgId")!);
            serializer.Serialize(writer, prop.GetValue(value));
        }
    }
}
