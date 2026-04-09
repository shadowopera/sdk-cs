#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Newtonsoft.Json converter for Duration serialization and deserialization.
    /// </summary>
    public class DurationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Duration);

        /// <summary>
        /// Deserializes JSON array or null to Duration (null/empty → Duration.Zero).
        /// </summary>
        /// <exception cref="JsonSerializationException">Thrown if JSON format is invalid.</exception>
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

        /// <summary>
        /// Serializes Duration to JSON array or null.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var duration = (Duration)value!;
            var shards = Archmage.ShardDuration(duration);
            if (shards is null)
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
