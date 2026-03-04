using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// JSON converter for Duration that serializes to compact array format [type, value].
    /// Zero duration is serialized as null.
    /// </summary>
    public class DurationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Duration);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return Duration.Zero;

            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonSerializationException($"Expected array for Duration, got {reader.TokenType}");

            var shards = new List<long>();
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                if (reader.TokenType != JsonToken.Integer)
                    throw new JsonSerializationException($"Expected number in Duration array, got {reader.TokenType}");

                shards.Add(Convert.ToInt64(reader.Value));
            }

            if (shards.Count == 0)
                return Duration.Zero;

            return Archmage.ParseDurationShards(shards.ToArray());
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var duration = (Duration)value!;
            var shards = Archmage.ShardDuration(duration);
            if (shards == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            foreach (var shard in shards)
            {
                writer.WriteValue(shard);
            }
            writer.WriteEndArray();
        }
    }
}
