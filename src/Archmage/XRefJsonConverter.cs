using System;
using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Newtonsoft.Json converter for XRef types that handles serialization and deserialization.
    /// </summary>
    /// <remarks>
    /// <para>This converter only serializes/deserializes the RawValue property.
    /// The Ref property is never serialized to JSON and is ignored during deserialization.
    /// The Ref property should be populated during the reference binding phase (via Atlas.BindRefs).</para>
    /// </remarks>
    public class XRefJsonConverter : JsonConverter
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo> RawValueCache = new();

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(XRef<,>);
        }

        /// <summary>
        /// Deserializes JSON as RawValue; Ref set to null (bound later via BindRefs).
        /// </summary>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return Activator.CreateInstance(objectType);

            Type keyType = objectType.GetGenericArguments()[0];
            var rawValue = serializer.Deserialize(reader, keyType)!;
            return Activator.CreateInstance(objectType, rawValue, null);
        }

        /// <summary>
        /// Serializes XRef as RawValue only (Ref property ignored).
        /// </summary>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var prop = RawValueCache.GetOrAdd(value!.GetType(), t => t.GetProperty("RawValue")!);
            serializer.Serialize(writer, prop.GetValue(value));
        }
    }
}
