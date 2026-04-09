#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Represents a duration as a signed number of nanoseconds.
    /// </summary>
    /// <remarks>
    /// <para>Duration provides methods to retrieve duration in various units (nanoseconds, microseconds, milliseconds, seconds, minutes, hours)
    /// and supports arithmetic operations, comparisons, and JSON serialization.</para>
    /// <para>The struct uses nanosecond precision internally and includes static readonly constants for common durations.
    /// The ToString method formats durations in a human-readable format like "1h30m5s".</para>
    /// <para>Duration serializes to and from a compact JSON array format for efficient storage.</para>
    /// </remarks>
    [JsonConverter(typeof(DurationJsonConverter))]
    public readonly struct Duration : IEquatable<Duration>, IComparable<Duration>
    {
        readonly long _nanoseconds;

        public static readonly Duration Zero = new(0);
        public static readonly Duration Nanosecond = new(1);
        public static readonly Duration Microsecond = new(1_000);
        public static readonly Duration Millisecond = new(1_000_000);
        public static readonly Duration Second = new(1_000_000_000);
        public static readonly Duration Minute = new(60_000_000_000);
        public static readonly Duration Hour = new(3_600_000_000_000);

        public Duration(long nanoseconds)
        {
            _nanoseconds = nanoseconds;
        }

        public static implicit operator long(Duration d) => d._nanoseconds;

        public static implicit operator Duration(long nanoseconds) => new(nanoseconds);

        /// <summary>
        /// Returns the absolute value of the duration.
        /// </summary>
        public Duration Abs() => new(Math.Abs(_nanoseconds));

        /// <summary>
        /// Returns floating-point duration in hours (may lose precision).
        /// </summary>
        public double Hours() => _nanoseconds / 1_000_000_000.0 / 3600.0;

        /// <summary>
        /// Returns floating-point duration in minutes (may lose precision).
        /// </summary>
        public double Minutes() => _nanoseconds / 1_000_000_000.0 / 60.0;

        /// <summary>
        /// Returns floating-point duration in seconds (may lose precision).
        /// </summary>
        public double Seconds() => _nanoseconds / 1_000_000_000.0;

        /// <summary>
        /// Returns integer millisecond count (truncates sub-millisecond precision).
        /// </summary>
        public long Milliseconds() => _nanoseconds / 1_000_000;

        /// <summary>
        /// Returns integer microsecond count (truncates sub-microsecond precision).
        /// </summary>
        public long Microseconds() => _nanoseconds / 1_000;

        public long Nanoseconds() => _nanoseconds;

        /// <summary>
        /// Returns the result of truncating this duration toward zero to the nearest multiple of the specified duration.
        /// </summary>
        /// <param name="m">The divisor duration. If zero or negative, this duration is returned unchanged.</param>
        /// <returns>A Duration truncated to a multiple of m.</returns>
        public Duration Truncate(Duration m)
        {
            long mNs = m._nanoseconds;
            if (mNs <= 0)
                return this;

            return new Duration(_nanoseconds - _nanoseconds % mNs);
        }

        /// <summary>
        /// Returns the result of rounding this duration to the nearest multiple of the specified duration.
        /// </summary>
        /// <param name="m">The divisor duration. If zero or negative, this duration is returned unchanged.</param>
        /// <returns>A Duration rounded to the nearest multiple of m.</returns>
        public Duration Round(Duration m)
        {
            long mNs = m._nanoseconds;
            if (mNs <= 0)
                return this;

            long r = _nanoseconds % mNs;
            if (_nanoseconds < 0)
            {
                r = -r;
                if (r + r < mNs)
                    return new Duration(_nanoseconds + r);
                if (_nanoseconds + mNs - r >= 0)
                    return new Duration(_nanoseconds + mNs - r);
                return Duration.Nanosecond * (1L << 63);
            }

            if (r + r < mNs)
                return new Duration(_nanoseconds - r);
            return new Duration(_nanoseconds + mNs - r);
        }

        /// <summary>
        /// Formats the duration as a human-readable string.
        /// </summary>
        /// <remarks>
        /// <para>Examples: "1h30m5s", "500ms", "1us", "-2h15m".</para>
        /// </remarks>
        /// <returns>A formatted duration string.</returns>
        public override string ToString()
        {
            if (_nanoseconds == 0)
                return "0s";

            var result = new System.Text.StringBuilder();
            var remaining = Math.Abs(_nanoseconds);

            if (_nanoseconds < 0)
                result.Append('-');

            // Hours
            if (remaining >= 3_600_000_000_000)
            {
                var hours = remaining / 3_600_000_000_000;
                result.Append(hours).Append('h');
                remaining %= 3_600_000_000_000;
            }

            // Minutes
            if (remaining >= 60_000_000_000)
            {
                var minutes = remaining / 60_000_000_000;
                result.Append(minutes).Append('m');
                remaining %= 60_000_000_000;
            }

            // Seconds
            if (remaining >= 1_000_000_000)
            {
                var seconds = remaining / 1_000_000_000;
                result.Append(seconds).Append('s');
                remaining %= 1_000_000_000;
            }

            // Milliseconds
            if (remaining >= 1_000_000)
            {
                var milliseconds = remaining / 1_000_000;
                result.Append(milliseconds).Append("ms");
                remaining %= 1_000_000;
            }

            // Microseconds
            if (remaining >= 1_000)
            {
                var microseconds = remaining / 1_000;
                result.Append(microseconds).Append("us");
                remaining %= 1_000;
            }

            // Nanoseconds
            if (remaining > 0)
            {
                result.Append(remaining).Append("ns");
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts to TimeSpan (100-nanosecond precision; may lose sub-tick precision).
        /// </summary>
        public TimeSpan ToTimeSpan() => TimeSpan.FromTicks(_nanoseconds / 100);

        /// <summary>
        /// Creates Duration from TimeSpan.
        /// </summary>
        public static Duration FromTimeSpan(TimeSpan timeSpan) => new(timeSpan.Ticks * 100);

        public int CompareTo(Duration other) => _nanoseconds.CompareTo(other._nanoseconds);

        public bool Equals(Duration other) => _nanoseconds == other._nanoseconds;

        public override bool Equals(object? obj) => obj is Duration other && Equals(other);

        public override int GetHashCode() => _nanoseconds.GetHashCode();

        public static Duration operator +(Duration left, Duration right) => new(left._nanoseconds + right._nanoseconds);

        public static Duration operator -(Duration left, Duration right) => new(left._nanoseconds - right._nanoseconds);

        public static Duration operator -(Duration d) => new(-d._nanoseconds);

        public static Duration operator *(Duration d, long scalar) => new(d._nanoseconds * scalar);

        public static Duration operator *(long scalar, Duration d) => new(scalar * d._nanoseconds);

        public static Duration operator /(Duration d, long scalar) => new(d._nanoseconds / scalar);

        public static long operator /(Duration left, Duration right) => left._nanoseconds / right._nanoseconds;

        public static Duration operator %(Duration left, Duration right) => new(left._nanoseconds % right._nanoseconds);

        public static bool operator ==(Duration left, Duration right) => left.Equals(right);

        public static bool operator !=(Duration left, Duration right) => !left.Equals(right);

        public static bool operator <(Duration left, Duration right) => left._nanoseconds < right._nanoseconds;

        public static bool operator <=(Duration left, Duration right) => left._nanoseconds <= right._nanoseconds;

        public static bool operator >(Duration left, Duration right) => left._nanoseconds > right._nanoseconds;

        public static bool operator >=(Duration left, Duration right) => left._nanoseconds >= right._nanoseconds;
    }
}
