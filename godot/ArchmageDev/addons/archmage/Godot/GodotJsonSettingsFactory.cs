#nullable enable

using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Factory for creating JsonSerializerSettings pre-configured with Godot vector converters.
    /// </summary>
    public static class GodotJsonSettingsFactory
    {
        /// <summary>
        /// Creates JsonSerializerSettings pre-configured with Godot vector converters.
        /// Supports: Vector2, Vector3, Vector4, Vector2I, Vector3I, Vector4I.
        /// Each vector type serializes to/from a JSON object: {"x": x, "y": y} etc.
        /// </summary>
        /// <param name="baseSettings">Optional base settings to clone and extend. If null, new settings are created.</param>
        /// <returns>JsonSerializerSettings with Godot vector converters registered.</returns>
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

                // The loop copies the reference to the caller's list; copy the list so that adding converters
                // leaves the caller's settings unchanged.
                settings.Converters = new List<JsonConverter>(baseSettings.Converters);
            }

            settings.Converters.Add(new GodotVector2JsonConverter());
            settings.Converters.Add(new GodotVector3JsonConverter());
            settings.Converters.Add(new GodotVector4JsonConverter());
            settings.Converters.Add(new GodotVector2IJsonConverter());
            settings.Converters.Add(new GodotVector3IJsonConverter());
            settings.Converters.Add(new GodotVector4IJsonConverter());
            return settings;
        }
    }

    /// <summary>
    /// JSON converter for Godot.Vector2.
    /// Serializes to/from {"x": x, "y": y} object format.
    /// </summary>
    public class GodotVector2JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Vector2);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec2<float>>(reader);
            return new Vector2(v.X, v.Y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (Vector2)value!;
            serializer.Serialize(writer, new Vec2<float>(vec.X, vec.Y));
        }
    }

    /// <summary>
    /// JSON converter for Godot.Vector3.
    /// Serializes to/from {"x": x, "y": y, "z": z} object format.
    /// </summary>
    public class GodotVector3JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Vector3);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec3<float>>(reader);
            return new Vector3(v.X, v.Y, v.Z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (Vector3)value!;
            serializer.Serialize(writer, new Vec3<float>(vec.X, vec.Y, vec.Z));
        }
    }

    /// <summary>
    /// JSON converter for Godot.Vector4.
    /// Serializes to/from {"x": x, "y": y, "z": z, "w": w} object format.
    /// </summary>
    public class GodotVector4JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Vector4);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec4<float>>(reader);
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (Vector4)value!;
            serializer.Serialize(writer, new Vec4<float>(vec.X, vec.Y, vec.Z, vec.W));
        }
    }

    /// <summary>
    /// JSON converter for Godot.Vector2I.
    /// Serializes to/from {"x": x, "y": y} object format.
    /// </summary>
    public class GodotVector2IJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Vector2I);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec2<int>>(reader);
            return new Vector2I(v.X, v.Y);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (Vector2I)value!;
            serializer.Serialize(writer, new Vec2<int>(vec.X, vec.Y));
        }
    }

    /// <summary>
    /// JSON converter for Godot.Vector3I.
    /// Serializes to/from {"x": x, "y": y, "z": z} object format.
    /// </summary>
    public class GodotVector3IJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Vector3I);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec3<int>>(reader);
            return new Vector3I(v.X, v.Y, v.Z);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (Vector3I)value!;
            serializer.Serialize(writer, new Vec3<int>(vec.X, vec.Y, vec.Z));
        }
    }

    /// <summary>
    /// JSON converter for Godot.Vector4I.
    /// Serializes to/from {"x": x, "y": y, "z": z, "w": w} object format.
    /// </summary>
    public class GodotVector4IJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Vector4I);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var v = serializer.Deserialize<Vec4<int>>(reader);
            return new Vector4I(v.X, v.Y, v.Z, v.W);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vec = (Vector4I)value!;
            serializer.Serialize(writer, new Vec4<int>(vec.X, vec.Y, vec.Z, vec.W));
        }
    }
}
