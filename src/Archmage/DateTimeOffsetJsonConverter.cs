#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Newtonsoft.Json converter for DateTimeOffset.
    /// </summary>
    public class DateTimeOffsetJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(DateTimeOffset);

        /// <summary>
        /// Deserializes JSON to DateTimeOffset (null → default).
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(DateTimeOffset);

            if (reader.TokenType == JsonToken.Date)
                return (DateTimeOffset)reader.Value!;

            var s = (string)reader.Value!;
            return DateTimeOffset.Parse(s);
        }

        /// <summary>
        /// Serializes DateTimeOffset to JSON (default → null).
        /// </summary>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var dto = (DateTimeOffset)value!;
            if (dto == default)
                writer.WriteNull();
            else
                writer.WriteValue(dto);
        }
    }
}
