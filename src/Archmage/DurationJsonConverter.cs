using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shadop.Archmage;

/// <summary>
/// JSON converter for Duration that serializes to compact array format [type, value].
/// Zero duration is serialized as null.
/// </summary>
public class DurationJsonConverter : JsonConverter<Duration>
{
    public override Duration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return Duration.Zero;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected array for Duration, got {reader.TokenType}");

        var shards = new List<long>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException($"Expected number in Duration array, got {reader.TokenType}");

            shards.Add(reader.GetInt64());
        }

        if (shards.Count == 0)
            return Duration.Zero;

        return Archmage.ParseDurationShards(shards.ToArray());
    }

    public override void Write(Utf8JsonWriter writer, Duration value, JsonSerializerOptions options)
    {
        var shards = Archmage.ShardDuration(value);
        if (shards == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var shard in shards)
        {
            writer.WriteNumberValue(shard);
        }
        writer.WriteEndArray();
    }
}
