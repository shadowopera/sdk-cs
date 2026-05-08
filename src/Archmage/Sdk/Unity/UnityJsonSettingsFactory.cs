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
    /// </summary>
    public class UnityVector2JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(UnityEngine.Vector2);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec2<float>>(reader);
            return new UnityEngine.Vector2(v.X, v.Y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (UnityEngine.Vector2)value!;
            serializer.Serialize(writer, new Vec2<float>(vec.x, vec.y));
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector3.
    /// Serializes to/from {"x": x, "y": y, "z": z} object format.
    /// </summary>
    public class UnityVector3JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(UnityEngine.Vector3);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec3<float>>(reader);
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (UnityEngine.Vector3)value!;
            serializer.Serialize(writer, new Vec3<float>(vec.x, vec.y, vec.z));
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector4.
    /// Serializes to/from {"x": x, "y": y, "z": z, "w": w} object format.
    /// </summary>
    public class UnityVector4JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(UnityEngine.Vector4);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec4<float>>(reader);
            return new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (UnityEngine.Vector4)value!;
            serializer.Serialize(writer, new Vec4<float>(vec.x, vec.y, vec.z, vec.w));
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector2Int.
    /// Serializes to/from {"x": x, "y": y} object format.
    /// </summary>
    public class UnityVector2IntJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(UnityEngine.Vector2Int);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec2<int>>(reader);
            return new UnityEngine.Vector2Int(v.X, v.Y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (UnityEngine.Vector2Int)value!;
            serializer.Serialize(writer, new Vec2<int>(vec.x, vec.y));
        }
    }

    /// <summary>
    /// JSON converter for UnityEngine.Vector3Int.
    /// Serializes to/from {"x": x, "y": y, "z": z} object format.
    /// </summary>
    public class UnityVector3IntJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(UnityEngine.Vector3Int);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec3<int>>(reader);
            return new UnityEngine.Vector3Int(v.X, v.Y, v.Z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (UnityEngine.Vector3Int)value!;
            serializer.Serialize(writer, new Vec3<int>(vec.x, vec.y, vec.z));
        }
    }
}

#endif
