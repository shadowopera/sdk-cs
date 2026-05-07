#nullable enable

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    static class VecReflectionCache
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo[]> Cache = new();

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
    /// JSON converter for Vec2 that serializes to/from {"x": x, "y": y} object.
    /// JSON null deserializes to a zero Vec2.
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

            Type elementType = objectType.GetGenericArguments()[0];
            object zero = Activator.CreateInstance(elementType)!;
            object x = zero, y = zero;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = serializer.Deserialize(reader, elementType)!;
                else if (prop == "y") y = serializer.Deserialize(reader, elementType)!;
                else reader.Skip();
            }

            return Activator.CreateInstance(objectType, x, y)!;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Type objectType = value!.GetType();
            var props = VecReflectionCache.GetProperties(objectType, "X", "Y");

            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, props[0].GetValue(value));
            writer.WritePropertyName("y"); serializer.Serialize(writer, props[1].GetValue(value));
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// JSON converter for Vec3 that serializes to/from {"x": x, "y": y, "z": z} object.
    /// JSON null deserializes to a zero Vec3.
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

            Type elementType = objectType.GetGenericArguments()[0];
            object zero = Activator.CreateInstance(elementType)!;
            object x = zero, y = zero, z = zero;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = serializer.Deserialize(reader, elementType)!;
                else if (prop == "y") y = serializer.Deserialize(reader, elementType)!;
                else if (prop == "z") z = serializer.Deserialize(reader, elementType)!;
                else reader.Skip();
            }

            return Activator.CreateInstance(objectType, x, y, z)!;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Type objectType = value!.GetType();
            var props = VecReflectionCache.GetProperties(objectType, "X", "Y", "Z");

            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, props[0].GetValue(value));
            writer.WritePropertyName("y"); serializer.Serialize(writer, props[1].GetValue(value));
            writer.WritePropertyName("z"); serializer.Serialize(writer, props[2].GetValue(value));
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// JSON converter for Vec4 that serializes to/from {"x": x, "y": y, "z": z, "w": w} object.
    /// JSON null deserializes to a zero Vec4.
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

            Type elementType = objectType.GetGenericArguments()[0];
            object zero = Activator.CreateInstance(elementType)!;
            object x = zero, y = zero, z = zero, w = zero;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = serializer.Deserialize(reader, elementType)!;
                else if (prop == "y") y = serializer.Deserialize(reader, elementType)!;
                else if (prop == "z") z = serializer.Deserialize(reader, elementType)!;
                else if (prop == "w") w = serializer.Deserialize(reader, elementType)!;
                else reader.Skip();
            }

            return Activator.CreateInstance(objectType, x, y, z, w)!;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Type objectType = value!.GetType();
            var props = VecReflectionCache.GetProperties(objectType, "X", "Y", "Z", "W");

            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, props[0].GetValue(value));
            writer.WritePropertyName("y"); serializer.Serialize(writer, props[1].GetValue(value));
            writer.WritePropertyName("z"); serializer.Serialize(writer, props[2].GetValue(value));
            writer.WritePropertyName("w"); serializer.Serialize(writer, props[3].GetValue(value));
            writer.WriteEndObject();
        }
    }
}
