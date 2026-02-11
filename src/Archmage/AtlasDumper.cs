using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadop.Archmage;

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
    /// <param name="options">Optional JSON serializer options. If null, uses default options with custom converters.</param>
    public static void DumpAtlas(IAtlas atlas, string outputDir, JsonSerializerOptions? options = null)
    {
        if (atlas == null)
            throw new ArgumentNullException(nameof(atlas));
        if (string.IsNullOrWhiteSpace(outputDir))
            throw new ArgumentException("Output directory cannot be empty.", nameof(outputDir));

        // Setup default options with custom converters
        options ??= CreateDumpOptions();

        var items = atlas.AtlasItems();

        foreach (var kvp in items)
        {
            var key = kvp.Key;
            var item = kvp.Value;

            // Skip items that are not ready or have no configuration
            if (!item.Ready || item.Cfg == null)
                continue;

            // Serialize item to JSON
            var json = JsonSerializer.Serialize(item.Cfg, options);

            // Write to file
            var filePath = Path.Combine(outputDir, key + ".json");
            var dir = Path.GetDirectoryName(filePath);
            if (dir != null)
                Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, json + "\n");
        }
    }

    private static JsonSerializerOptions CreateDumpOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = null // Preserve property names as-is
        };

        // Register custom converters (zero-value Vec writes null, matching Go behavior)
        options.Converters.Add(new DurationJsonConverter());
        options.Converters.Add(new ZeroVecNullVec2JsonConverterFactory());
        options.Converters.Add(new ZeroVecNullVec3JsonConverterFactory());
        options.Converters.Add(new ZeroVecNullVec4JsonConverterFactory());
        options.Converters.Add(new RefJsonConverterFactory());

        return options;
    }
}

/// <summary>
/// Vec2 converter factory for DumpAtlas that writes null for zero-valued vectors.
/// </summary>
internal class ZeroVecNullVec2JsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Vec2<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ZeroVecNullVec2JsonConverter<>).MakeGenericType(elementType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

internal class ZeroVecNullVec2JsonConverter<T> : JsonConverter<Vec2<T>>
    where T : IEquatable<T>
{
    public override Vec2<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var inner = new Vec2JsonConverter<T>();
        return inner.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, Vec2<T> value, JsonSerializerOptions options)
    {
        if (EqualityComparer<T>.Default.Equals(value.X, default!) &&
            EqualityComparer<T>.Default.Equals(value.Y, default!))
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.X, options);
        JsonSerializer.Serialize(writer, value.Y, options);
        writer.WriteEndArray();
    }
}

/// <summary>
/// Vec3 converter factory for DumpAtlas that writes null for zero-valued vectors.
/// </summary>
internal class ZeroVecNullVec3JsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Vec3<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ZeroVecNullVec3JsonConverter<>).MakeGenericType(elementType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

internal class ZeroVecNullVec3JsonConverter<T> : JsonConverter<Vec3<T>>
    where T : IEquatable<T>
{
    public override Vec3<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var inner = new Vec3JsonConverter<T>();
        return inner.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, Vec3<T> value, JsonSerializerOptions options)
    {
        if (EqualityComparer<T>.Default.Equals(value.X, default!) &&
            EqualityComparer<T>.Default.Equals(value.Y, default!) &&
            EqualityComparer<T>.Default.Equals(value.Z, default!))
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.X, options);
        JsonSerializer.Serialize(writer, value.Y, options);
        JsonSerializer.Serialize(writer, value.Z, options);
        writer.WriteEndArray();
    }
}

/// <summary>
/// Vec4 converter factory for DumpAtlas that writes null for zero-valued vectors.
/// </summary>
internal class ZeroVecNullVec4JsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Vec4<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ZeroVecNullVec4JsonConverter<>).MakeGenericType(elementType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

internal class ZeroVecNullVec4JsonConverter<T> : JsonConverter<Vec4<T>>
    where T : IEquatable<T>
{
    public override Vec4<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var inner = new Vec4JsonConverter<T>();
        return inner.Read(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, Vec4<T> value, JsonSerializerOptions options)
    {
        if (EqualityComparer<T>.Default.Equals(value.X, default!) &&
            EqualityComparer<T>.Default.Equals(value.Y, default!) &&
            EqualityComparer<T>.Default.Equals(value.Z, default!) &&
            EqualityComparer<T>.Default.Equals(value.W, default!))
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.X, options);
        JsonSerializer.Serialize(writer, value.Y, options);
        JsonSerializer.Serialize(writer, value.Z, options);
        JsonSerializer.Serialize(writer, value.W, options);
        writer.WriteEndArray();
    }
}
