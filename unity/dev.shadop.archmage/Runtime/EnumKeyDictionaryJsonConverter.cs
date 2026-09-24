#nullable enable

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Newtonsoft.Json converter that writes enum-keyed dictionaries with numeric keys,
    /// the same keys archmage exports. Reading uses the default handling, which accepts numeric keys.
    /// </summary>
    public class EnumKeyDictionaryJsonConverter : JsonConverter
    {
        static readonly ConcurrentDictionary<Type, bool> _cache = new();

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType) => _cache.GetOrAdd(objectType, HasEnumKey);

        static bool HasEnumKey(Type type)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return iface.GetGenericArguments()[0].IsEnum;
            }
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the dictionary, writing each key as the enum's underlying integer value.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (DictionaryEntry entry in (IDictionary)value!)
            {
                var key = Convert.ChangeType(entry.Key, Enum.GetUnderlyingType(entry.Key.GetType()), CultureInfo.InvariantCulture);
                writer.WritePropertyName(Convert.ToString(key, CultureInfo.InvariantCulture)!);
                serializer.Serialize(writer, entry.Value);
            }
            writer.WriteEndObject();
        }
    }
}
