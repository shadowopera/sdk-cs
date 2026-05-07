#nullable enable

#if UNITY_5_3_OR_NEWER

using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Factory for creating JsonSerializerSettings pre-configured with Unity vector converters.
    /// </summary>
    public static class UnityJsonSettingsFactory
    {
        /// <summary>
        /// Creates JsonSerializerSettings pre-configured with Unity vector converters.
        /// Supports: Vector2, Vector3, Vector4, Vector2Int, Vector3Int.
        /// Each vector type serializes to/from a JSON object: {"x": x, "y": y} etc.
        /// </summary>
        /// <param name="baseSettings">Optional base settings to clone and extend. If null, new settings are created.</param>
        /// <returns>JsonSerializerSettings with Unity vector converters registered.</returns>
        public static JsonSerializerSettings Create(JsonSerializerSettings? baseSettings = null)
        {
            var settings = new JsonSerializerSettings();
            if (baseSettings is not null)
            {
                foreach (var prop in typeof(JsonSerializerSettings).GetProperties())
                {
                    if (prop.CanWrite)
                        prop.SetValue(settings, prop.GetValue(baseSettings));
                }
            }

            settings.Converters.Add(new UnityVector2JsonConverter());
            settings.Converters.Add(new UnityVector3JsonConverter());
            settings.Converters.Add(new UnityVector4JsonConverter());
            settings.Converters.Add(new UnityVector2IntJsonConverter());
            settings.Converters.Add(new UnityVector3IntJsonConverter());
            return settings;
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector2.
    /// Serializes to/from {"x": x, "y": y} object format.
    /// JSON null deserializes to Vector2.zero.
    /// </summary>
    public class UnityVector2JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector2);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return UnityEngine.Vector2.zero;

            float x = 0, y = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = (float)serializer.Deserialize(reader, typeof(float))!;
                else if (prop == "y") y = (float)serializer.Deserialize(reader, typeof(float))!;
                else reader.Skip();
            }
            return new UnityEngine.Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null) { writer.WriteNull(); return; }
            var vec = (UnityEngine.Vector2)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, vec.x);
            writer.WritePropertyName("y"); serializer.Serialize(writer, vec.y);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector3.
    /// Serializes to/from {"x": x, "y": y, "z": z} object format.
    /// JSON null deserializes to Vector3.zero.
    /// </summary>
    public class UnityVector3JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector3);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return UnityEngine.Vector3.zero;

            float x = 0, y = 0, z = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = (float)serializer.Deserialize(reader, typeof(float))!;
                else if (prop == "y") y = (float)serializer.Deserialize(reader, typeof(float))!;
                else if (prop == "z") z = (float)serializer.Deserialize(reader, typeof(float))!;
                else reader.Skip();
            }
            return new UnityEngine.Vector3(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null) { writer.WriteNull(); return; }
            var vec = (UnityEngine.Vector3)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, vec.x);
            writer.WritePropertyName("y"); serializer.Serialize(writer, vec.y);
            writer.WritePropertyName("z"); serializer.Serialize(writer, vec.z);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector4.
    /// Serializes to/from {"x": x, "y": y, "z": z, "w": w} object format.
    /// JSON null deserializes to Vector4.zero.
    /// </summary>
    public class UnityVector4JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector4);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return UnityEngine.Vector4.zero;

            float x = 0, y = 0, z = 0, w = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = (float)serializer.Deserialize(reader, typeof(float))!;
                else if (prop == "y") y = (float)serializer.Deserialize(reader, typeof(float))!;
                else if (prop == "z") z = (float)serializer.Deserialize(reader, typeof(float))!;
                else if (prop == "w") w = (float)serializer.Deserialize(reader, typeof(float))!;
                else reader.Skip();
            }
            return new UnityEngine.Vector4(x, y, z, w);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null) { writer.WriteNull(); return; }
            var vec = (UnityEngine.Vector4)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, vec.x);
            writer.WritePropertyName("y"); serializer.Serialize(writer, vec.y);
            writer.WritePropertyName("z"); serializer.Serialize(writer, vec.z);
            writer.WritePropertyName("w"); serializer.Serialize(writer, vec.w);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector2Int.
    /// Serializes to/from {"x": x, "y": y} object format.
    /// JSON null deserializes to Vector2Int.zero.
    /// </summary>
    public class UnityVector2IntJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector2Int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return UnityEngine.Vector2Int.zero;

            int x = 0, y = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = (int)serializer.Deserialize(reader, typeof(int))!;
                else if (prop == "y") y = (int)serializer.Deserialize(reader, typeof(int))!;
                else reader.Skip();
            }
            return new UnityEngine.Vector2Int(x, y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null) { writer.WriteNull(); return; }
            var vec = (UnityEngine.Vector2Int)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, vec.x);
            writer.WritePropertyName("y"); serializer.Serialize(writer, vec.y);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector3Int.
    /// Serializes to/from {"x": x, "y": y, "z": z} object format.
    /// JSON null deserializes to Vector3Int.zero.
    /// </summary>
    public class UnityVector3IntJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector3Int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return UnityEngine.Vector3Int.zero;

            int x = 0, y = 0, z = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string prop = ((string)reader.Value!).ToLowerInvariant();
                reader.Read();
                if (prop == "x") x = (int)serializer.Deserialize(reader, typeof(int))!;
                else if (prop == "y") y = (int)serializer.Deserialize(reader, typeof(int))!;
                else if (prop == "z") z = (int)serializer.Deserialize(reader, typeof(int))!;
                else reader.Skip();
            }
            return new UnityEngine.Vector3Int(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null) { writer.WriteNull(); return; }
            var vec = (UnityEngine.Vector3Int)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x"); serializer.Serialize(writer, vec.x);
            writer.WritePropertyName("y"); serializer.Serialize(writer, vec.y);
            writer.WritePropertyName("z"); serializer.Serialize(writer, vec.z);
            writer.WriteEndObject();
        }
    }
}

#endif
