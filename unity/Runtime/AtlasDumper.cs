using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Writes all loaded atlas items to JSON files in outputDir.
        /// Each item is written to a separate file named {key}.json.
        /// </summary>
        /// <param name="atlas">The Atlas instance to dump.</param>
        /// <param name="outputDir">The directory to write JSON files to.</param>
        /// <param name="settings">Optional JSON serializer settings. If null, uses default settings with custom converters.</param>
        public static void DumpAtlas(IAtlas atlas, string outputDir, JsonSerializerSettings? settings = null)
        {
            if (atlas == null)
                throw new ArgumentNullException(nameof(atlas));
            if (string.IsNullOrWhiteSpace(outputDir))
                throw new ArgumentException("Output directory cannot be empty.", nameof(outputDir));

            // Setup default settings with custom converters
            settings ??= CreateDumpSettings();

            var items = atlas.AtlasItems();

            foreach (var kvp in items)
            {
                var key = kvp.Key;
                var item = kvp.Value;

                // Skip items that are not ready or have no configuration
                if (!item.Ready || item.Cfg == null)
                    continue;

                // Serialize item to JSON
                var json = JsonConvert.SerializeObject(item.Cfg, settings);

                // Write to file
                var filePath = Path.Combine(outputDir, key + ".json");
                var dir = Path.GetDirectoryName(filePath);
                if (dir != null)
                    Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, json + "\n");
            }
        }

        private static JsonSerializerSettings CreateDumpSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            // Register custom converters (zero-value Vec writes null, matching Go behavior)
            settings.Converters.Add(new DurationJsonConverter());
            settings.Converters.Add(new ZeroVecNullVec2JsonConverter());
            settings.Converters.Add(new ZeroVecNullVec3JsonConverter());
            settings.Converters.Add(new ZeroVecNullVec4JsonConverter());
            settings.Converters.Add(new RefJsonConverter());

            return settings;
        }
    }

    /// <summary>
    /// Vec2 converter for DumpAtlas that writes null for zero-valued vectors.
    /// </summary>
    internal class ZeroVecNullVec2JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(Vec2<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var inner = new Vec2JsonConverter();
            return inner.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var objectType = value!.GetType();
            var elementType = objectType.GetGenericArguments()[0];
            var props = VecReflectionCache.GetProperties(objectType, "X", "Y");
            var x = props[0].GetValue(value);
            var y = props[1].GetValue(value);

            if (ZeroVecHelper.IsDefault(x, elementType) && ZeroVecHelper.IsDefault(y, elementType))
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            serializer.Serialize(writer, x);
            serializer.Serialize(writer, y);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// Vec3 converter for DumpAtlas that writes null for zero-valued vectors.
    /// </summary>
    internal class ZeroVecNullVec3JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(Vec3<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var inner = new Vec3JsonConverter();
            return inner.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var objectType = value!.GetType();
            var elementType = objectType.GetGenericArguments()[0];
            var props = VecReflectionCache.GetProperties(objectType, "X", "Y", "Z");
            var x = props[0].GetValue(value);
            var y = props[1].GetValue(value);
            var z = props[2].GetValue(value);

            if (ZeroVecHelper.IsDefault(x, elementType) &&
                ZeroVecHelper.IsDefault(y, elementType) &&
                ZeroVecHelper.IsDefault(z, elementType))
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            serializer.Serialize(writer, x);
            serializer.Serialize(writer, y);
            serializer.Serialize(writer, z);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// Vec4 converter for DumpAtlas that writes null for zero-valued vectors.
    /// </summary>
    internal class ZeroVecNullVec4JsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(Vec4<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var inner = new Vec4JsonConverter();
            return inner.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var objectType = value!.GetType();
            var elementType = objectType.GetGenericArguments()[0];
            var props = VecReflectionCache.GetProperties(objectType, "X", "Y", "Z", "W");
            var x = props[0].GetValue(value);
            var y = props[1].GetValue(value);
            var z = props[2].GetValue(value);
            var w = props[3].GetValue(value);

            if (ZeroVecHelper.IsDefault(x, elementType) &&
                ZeroVecHelper.IsDefault(y, elementType) &&
                ZeroVecHelper.IsDefault(z, elementType) &&
                ZeroVecHelper.IsDefault(w, elementType))
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            serializer.Serialize(writer, x);
            serializer.Serialize(writer, y);
            serializer.Serialize(writer, z);
            serializer.Serialize(writer, w);
            writer.WriteEndArray();
        }
    }

    internal static class ZeroVecHelper
    {
        private static readonly ConcurrentDictionary<Type, object?> DefaultValueCache = new();

        internal static bool IsDefault(object? value, Type type)
        {
            if (value == null)
                return true;

            if (type.IsValueType)
            {
                var defaultValue = DefaultValueCache.GetOrAdd(type, t => Activator.CreateInstance(t));
                return value.Equals(defaultValue);
            }

            return false;
        }
    }
}
