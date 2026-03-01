using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadop.Archmage;

/// <summary>
/// JSON converter factory for Vec2.
/// </summary>
public class Vec2JsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Vec2<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type elementType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(Vec2JsonConverter<>).MakeGenericType(elementType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// JSON converter for Vec2 that serializes to/from [x, y] array.
/// </summary>
public class Vec2JsonConverter<T> : JsonConverter<Vec2<T>>
    where T : IEquatable<T>
{
    public override Vec2<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return default;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected array for Vec2, got {reader.TokenType}");

        reader.Read();
        T x = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        T y = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException("Expected end of array for Vec2");

        return new Vec2<T>(x, y);
    }

    public override void Write(Utf8JsonWriter writer, Vec2<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.X, options);
        JsonSerializer.Serialize(writer, value.Y, options);
        writer.WriteEndArray();
    }
}

/// <summary>
/// JSON converter factory for Vec3.
/// </summary>
public class Vec3JsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Vec3<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type elementType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(Vec3JsonConverter<>).MakeGenericType(elementType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// JSON converter for Vec3 that serializes to/from [x, y, z] array.
/// </summary>
public class Vec3JsonConverter<T> : JsonConverter<Vec3<T>>
    where T : IEquatable<T>
{
    public override Vec3<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return default;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected array for Vec3, got {reader.TokenType}");

        reader.Read();
        T x = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        T y = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        T z = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException("Expected end of array for Vec3");

        return new Vec3<T>(x, y, z);
    }

    public override void Write(Utf8JsonWriter writer, Vec3<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.X, options);
        JsonSerializer.Serialize(writer, value.Y, options);
        JsonSerializer.Serialize(writer, value.Z, options);
        writer.WriteEndArray();
    }
}

/// <summary>
/// JSON converter factory for Vec4.
/// </summary>
public class Vec4JsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Vec4<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type elementType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(Vec4JsonConverter<>).MakeGenericType(elementType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// JSON converter for Vec4 that serializes to/from [x, y, z, w] array.
/// </summary>
public class Vec4JsonConverter<T> : JsonConverter<Vec4<T>>
    where T : IEquatable<T>
{
    public override Vec4<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return default;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected array for Vec4, got {reader.TokenType}");

        reader.Read();
        T x = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        T y = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        T z = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        T w = JsonSerializer.Deserialize<T>(ref reader, options)!;

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException("Expected end of array for Vec4");

        return new Vec4<T>(x, y, z, w);
    }

    public override void Write(Utf8JsonWriter writer, Vec4<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.X, options);
        JsonSerializer.Serialize(writer, value.Y, options);
        JsonSerializer.Serialize(writer, value.Z, options);
        JsonSerializer.Serialize(writer, value.W, options);
        writer.WriteEndArray();
    }
}
