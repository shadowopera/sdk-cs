#nullable enable

#if UNITY_5_3_OR_NEWER

using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shadop.Archmage
{
    /// <summary>
    /// Factory for creating JsonSerializerSettings pre-configured with Unity vector converters.
    /// </summary>
    public static class UnityJsonSettingsFactory
    {
        /// <summary>
        /// Creates JsonSerializerSettings pre-configured with Unity vector converters.
        /// Supports: Vector2, Vector3, Vector4, Vector2Int, Vector3Int.
        /// Each vector type serializes to/from a JSON array: [x, y] or [x, y, z] or [x, y, z, w]
        /// </summary>
        /// <param name="baseSettings">Optional base settings to clone and extend. If null, new settings are created.</param>
        /// <returns>JsonSerializerSettings with Unity vector converters registered.</returns>
        public static JsonSerializerSettings Create(JsonSerializerSettings? baseSettings = null)
        {
            var settings = new JsonSerializerSettings();
            if (baseSettings != null)
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
    /// Serializes to/from [x, y] array format.
    /// Null values deserialize to Vector2.zero.
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

            var array = JArray.Load(reader);
            if (array.Count < 2)
                throw new JsonSerializationException($"Expected array with 2 elements for Vector2, got {array.Count}");

            float x = array[0].ToObject<float>(serializer);
            float y = array[1].ToObject<float>(serializer);

            return new UnityEngine.Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var vec = (UnityEngine.Vector2)value;
            writer.WriteStartArray();
            serializer.Serialize(writer, vec.x);
            serializer.Serialize(writer, vec.y);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector3.
    /// Serializes to/from [x, y, z] array format.
    /// Null values deserialize to Vector3.zero.
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

            var array = JArray.Load(reader);
            if (array.Count < 3)
                throw new JsonSerializationException($"Expected array with 3 elements for Vector3, got {array.Count}");

            float x = array[0].ToObject<float>(serializer);
            float y = array[1].ToObject<float>(serializer);
            float z = array[2].ToObject<float>(serializer);

            return new UnityEngine.Vector3(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var vec = (UnityEngine.Vector3)value;
            writer.WriteStartArray();
            serializer.Serialize(writer, vec.x);
            serializer.Serialize(writer, vec.y);
            serializer.Serialize(writer, vec.z);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector4.
    /// Serializes to/from [x, y, z, w] array format.
    /// Null values deserialize to Vector4.zero.
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

            var array = JArray.Load(reader);
            if (array.Count < 4)
                throw new JsonSerializationException($"Expected array with 4 elements for Vector4, got {array.Count}");

            float x = array[0].ToObject<float>(serializer);
            float y = array[1].ToObject<float>(serializer);
            float z = array[2].ToObject<float>(serializer);
            float w = array[3].ToObject<float>(serializer);

            return new UnityEngine.Vector4(x, y, z, w);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var vec = (UnityEngine.Vector4)value;
            writer.WriteStartArray();
            serializer.Serialize(writer, vec.x);
            serializer.Serialize(writer, vec.y);
            serializer.Serialize(writer, vec.z);
            serializer.Serialize(writer, vec.w);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector2Int.
    /// Serializes to/from [x, y] array format.
    /// Null values deserialize to Vector2Int.zero.
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

            var array = JArray.Load(reader);
            if (array.Count < 2)
                throw new JsonSerializationException($"Expected array with 2 elements for Vector2Int, got {array.Count}");

            int x = array[0].ToObject<int>(serializer);
            int y = array[1].ToObject<int>(serializer);

            return new UnityEngine.Vector2Int(x, y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var vec = (UnityEngine.Vector2Int)value;
            writer.WriteStartArray();
            serializer.Serialize(writer, vec.x);
            serializer.Serialize(writer, vec.y);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector3Int.
    /// Serializes to/from [x, y, z] array format.
    /// Null values deserialize to Vector3Int.zero.
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

            var array = JArray.Load(reader);
            if (array.Count < 3)
                throw new JsonSerializationException($"Expected array with 3 elements for Vector3Int, got {array.Count}");

            int x = array[0].ToObject<int>(serializer);
            int y = array[1].ToObject<int>(serializer);
            int z = array[2].ToObject<int>(serializer);

            return new UnityEngine.Vector3Int(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var vec = (UnityEngine.Vector3Int)value;
            writer.WriteStartArray();
            serializer.Serialize(writer, vec.x);
            serializer.Serialize(writer, vec.y);
            serializer.Serialize(writer, vec.z);
            writer.WriteEndArray();
        }
    }
}

#endif
