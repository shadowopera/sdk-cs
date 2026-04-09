#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Represents a color with red, green, blue, and alpha channels.
    /// </summary>
    /// <remarks>
    /// <para>Serializes to JSON as <c>"#RRGGBBAA"</c>. Deserializes from <c>"#RRGGBB"</c>,
    /// <c>"#RRGGBBAA"</c>, an empty string, or <c>null</c>.
    /// The zero value (all channels 0) serializes as an empty string.</para>
    /// </remarks>
    [JsonConverter(typeof(RgbaJsonConverter))]
    public readonly struct Rgba : IEquatable<Rgba>, IZero
    {
        private static readonly char[] HexUpper = "0123456789ABCDEF".ToCharArray();

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public Rgba(byte r, byte g, byte b, byte a = 0xFF)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Returns true if all channels are zero.
        /// </summary>
        public bool IsZero => R == 0 && G == 0 && B == 0 && A == 0;

        /// <summary>
        /// Parses a <c>"#RRGGBB"</c> or <c>"#RRGGBBAA"</c> hex string.
        /// An empty or null string returns the zero value.
        /// </summary>
        /// <exception cref="FormatException">Thrown when the string format is invalid.</exception>
        public static Rgba Parse(string? s)
        {
            if (!TryParse(s, out Rgba result))
                throw new FormatException($"<archmage> invalid Rgba string \"{s}\".");
            return result;
        }

        /// <summary>
        /// Tries to parse a <c>"#RRGGBB"</c> or <c>"#RRGGBBAA"</c> hex string.
        /// An empty or null string sets <paramref name="result"/> to the zero value and returns true.
        /// </summary>
        public static bool TryParse(string? s, out Rgba result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = default;
                return true;
            }

            if (s[0] != '#')
            {
                result = default;
                return false;
            }

            byte r, g, b, a;
            switch (s.Length)
            {
                case 7: // #RRGGBB
                    if (!TryUnhexByte(s[1], s[2], out r) ||
                        !TryUnhexByte(s[3], s[4], out g) ||
                        !TryUnhexByte(s[5], s[6], out b))
                    {
                        result = default;
                        return false;
                    }
                    a = 0xFF;
                    break;

                case 9: // #RRGGBBAA
                    if (!TryUnhexByte(s[1], s[2], out r) ||
                        !TryUnhexByte(s[3], s[4], out g) ||
                        !TryUnhexByte(s[5], s[6], out b) ||
                        !TryUnhexByte(s[7], s[8], out a))
                    {
                        result = default;
                        return false;
                    }
                    break;

                default:
                    result = default;
                    return false;
            }

            result = new Rgba(r, g, b, a);
            return true;
        }

        /// <summary>
        /// Returns the color as <c>"#RRGGBB"</c> when alpha is 0xFF, otherwise <c>"#RRGGBBAA"</c>.
        /// </summary>
        public override string ToString()
        {
            if (A == 0xFF)
            {
                return string.Create(7, this, (span, c) =>
                {
                    span[0] = '#';
                    span[1] = HexUpper[c.R >> 4]; span[2] = HexUpper[c.R & 0xF];
                    span[3] = HexUpper[c.G >> 4]; span[4] = HexUpper[c.G & 0xF];
                    span[5] = HexUpper[c.B >> 4]; span[6] = HexUpper[c.B & 0xF];
                });
            }
            else
            {
                return string.Create(9, this, (span, c) =>
                {
                    span[0] = '#';
                    span[1] = HexUpper[c.R >> 4]; span[2] = HexUpper[c.R & 0xF];
                    span[3] = HexUpper[c.G >> 4]; span[4] = HexUpper[c.G & 0xF];
                    span[5] = HexUpper[c.B >> 4]; span[6] = HexUpper[c.B & 0xF];
                    span[7] = HexUpper[c.A >> 4]; span[8] = HexUpper[c.A & 0xF];
                });
            }
        }

        public bool Equals(Rgba other) => R == other.R && G == other.G && B == other.B && A == other.A;
        public override bool Equals(object? obj) => obj is Rgba other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);
        public static bool operator ==(Rgba left, Rgba right) => left.Equals(right);
        public static bool operator !=(Rgba left, Rgba right) => !left.Equals(right);

        private static bool TryUnhexNibble(char c, out byte value)
        {
            if (c >= '0' && c <= '9') { value = (byte)(c - '0'); return true; }
            if (c >= 'a' && c <= 'f') { value = (byte)(c - 'a' + 10); return true; }
            if (c >= 'A' && c <= 'F') { value = (byte)(c - 'A' + 10); return true; }
            value = 0;
            return false;
        }

        private static bool TryUnhexByte(char hi, char lo, out byte value)
        {
            if (TryUnhexNibble(hi, out byte h) && TryUnhexNibble(lo, out byte l))
            {
                value = (byte)((h << 4) | l);
                return true;
            }
            value = 0;
            return false;
        }
    }
}
