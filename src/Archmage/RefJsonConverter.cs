using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;

namespace Shadop.Archmage;

/// <summary>
/// JSON converter for Ref that only serializes/deserializes the RawValue.
/// The Value property is ignored during JSON operations and should be populated
/// during the reference binding phase.
/// </summary>
public class RefJsonConverter : JsonConverter
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo> RawValueCache = new();

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               objectType.GetGenericTypeDefinition() == typeof(Ref<,>);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        Type keyType = objectType.GetGenericArguments()[0];
        object rawValue = serializer.Deserialize(reader, keyType)!;
        return Activator.CreateInstance(objectType, rawValue);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var prop = RawValueCache.GetOrAdd(value.GetType(), t => t.GetProperty("RawValue")!);
        serializer.Serialize(writer, prop.GetValue(value));
    }
}
