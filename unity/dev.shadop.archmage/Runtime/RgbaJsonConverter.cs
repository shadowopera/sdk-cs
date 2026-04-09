#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Newtonsoft.Json converter for <see cref="Rgba"/>.
    /// </summary>
    /// <remarks>
    /// <para>Serializes as <c>"#RRGGBBAA"</c>, or an empty string for the zero value.
    /// Deserializes from <c>"#RRGGBB"</c>, <c>"#RRGGBBAA"</c>, empty string, or <c>null</c>.</para>
    /// </remarks>
    public class RgbaJsonConverter : JsonConverter<Rgba>
    {
        public override Rgba ReadJson(JsonReader reader, Type objectType, Rgba existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return default;

                case JsonToken.String:
                    var s = (string?)reader.Value;
                    if (!Rgba.TryParse(s, out Rgba result))
                        throw new JsonSerializationException($"<archmage> invalid Rgba string \"{s}\".");
                    return result;

                default:
                    throw new JsonSerializationException(
                        $"<archmage> Rgba: expected string or null, got {reader.TokenType}.");
            }
        }

        public override void WriteJson(JsonWriter writer, Rgba value, JsonSerializer serializer)
        {
            switch (value.A)
            {
                case 0xFF:
                    writer.WriteValue(value.ToString());
                    break;

                case 0x00 when value.R == 0 && value.G == 0 && value.B == 0:
                    writer.WriteValue(string.Empty);
                    break;

                default:
                    writer.WriteValue(value.ToString());
                    break;
            }
        }
    }
}
