using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shadop.Archmage;

internal static class VecReflectionCache
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Cache = new();

    internal static PropertyInfo[] GetProperties(Type objectType, params string[] names)
    {
        return Cache.GetOrAdd(objectType, t =>
        {
            var props = new PropertyInfo[names.Length];
            for (int i = 0; i < names.Length; i++)
                props[i] = t.GetProperty(names[i])!;
            return props;
        });
    }
}

/// <summary>
/// JSON converter for Vec2 that serializes to/from [x, y] array.
/// </summary>
public class Vec2JsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               objectType.GetGenericTypeDefinition() == typeof(Vec2<>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return Activator.CreateInstance(objectType)!;

        var array = JArray.Load(reader);
        if (array.Count < 2)
            throw new JsonSerializationException($"Expected array with 2 elements for Vec2, got {array.Count}");

        Type elementType = objectType.GetGenericArguments()[0];
        object x = array[0].ToObject(elementType, serializer)!;
        object y = array[1].ToObject(elementType, serializer)!;

        return Activator.CreateInstance(objectType, x, y)!;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        Type objectType = value!.GetType();
        var props = VecReflectionCache.GetProperties(objectType, "X", "Y");

        writer.WriteStartArray();
        serializer.Serialize(writer, props[0].GetValue(value));
        serializer.Serialize(writer, props[1].GetValue(value));
        writer.WriteEndArray();
    }
}

/// <summary>
/// JSON converter for Vec3 that serializes to/from [x, y, z] array.
/// </summary>
public class Vec3JsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               objectType.GetGenericTypeDefinition() == typeof(Vec3<>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return Activator.CreateInstance(objectType)!;

        var array = JArray.Load(reader);
        if (array.Count < 3)
            throw new JsonSerializationException($"Expected array with 3 elements for Vec3, got {array.Count}");

        Type elementType = objectType.GetGenericArguments()[0];
        object x = array[0].ToObject(elementType, serializer)!;
        object y = array[1].ToObject(elementType, serializer)!;
        object z = array[2].ToObject(elementType, serializer)!;

        return Activator.CreateInstance(objectType, x, y, z)!;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        Type objectType = value!.GetType();
        var props = VecReflectionCache.GetProperties(objectType, "X", "Y", "Z");

        writer.WriteStartArray();
        serializer.Serialize(writer, props[0].GetValue(value));
        serializer.Serialize(writer, props[1].GetValue(value));
        serializer.Serialize(writer, props[2].GetValue(value));
        writer.WriteEndArray();
    }
}

/// <summary>
/// JSON converter for Vec4 that serializes to/from [x, y, z, w] array.
/// </summary>
public class Vec4JsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               objectType.GetGenericTypeDefinition() == typeof(Vec4<>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return Activator.CreateInstance(objectType)!;

        var array = JArray.Load(reader);
        if (array.Count < 4)
            throw new JsonSerializationException($"Expected array with 4 elements for Vec4, got {array.Count}");

        Type elementType = objectType.GetGenericArguments()[0];
        object x = array[0].ToObject(elementType, serializer)!;
        object y = array[1].ToObject(elementType, serializer)!;
        object z = array[2].ToObject(elementType, serializer)!;
        object w = array[3].ToObject(elementType, serializer)!;

        return Activator.CreateInstance(objectType, x, y, z, w)!;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        Type objectType = value!.GetType();
        var props = VecReflectionCache.GetProperties(objectType, "X", "Y", "Z", "W");

        writer.WriteStartArray();
        serializer.Serialize(writer, props[0].GetValue(value));
        serializer.Serialize(writer, props[1].GetValue(value));
        serializer.Serialize(writer, props[2].GetValue(value));
        serializer.Serialize(writer, props[3].GetValue(value));
        writer.WriteEndArray();
    }
}
