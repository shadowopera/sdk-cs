using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadop.Archmage;

/// <summary>
/// JSON converter factory for Ref that creates type-specific converters.
/// </summary>
public class RefJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Ref<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type[] genericArgs = typeToConvert.GetGenericArguments();
        Type keyType = genericArgs[0];
        Type valueType = genericArgs[1];

        Type converterType = typeof(RefJsonConverter<,>).MakeGenericType(keyType, valueType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// JSON converter for Ref that only serializes/deserializes the RawValue.
/// The Ref property is ignored during JSON operations and should be populated
/// during the reference binding phase.
/// </summary>
public class RefJsonConverter<TKey, TValue> : JsonConverter<Ref<TKey, TValue>>
    where TKey : notnull
    where TValue : class
{
    public override Ref<TKey, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        TKey rawValue = JsonSerializer.Deserialize<TKey>(ref reader, options)!;
        return new Ref<TKey, TValue>(rawValue);
    }

    public override void Write(Utf8JsonWriter writer, Ref<TKey, TValue> value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.RawValue, options);
    }
}
