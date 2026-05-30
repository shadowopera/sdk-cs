#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// A numeric range bounded by <see cref="Min"/> and <see cref="Max"/>. The <c>Sample</c>
    /// extension methods (in <see cref="MinMaxExtensions"/>) can draw a random value from it.
    /// </summary>
    public struct MinMax<T> : IEquatable<MinMax<T>> where T : unmanaged, IEquatable<T>
    {
        [JsonProperty("min")] public T Min { get; set; }
        [JsonProperty("max")] public T Max { get; set; }

        public MinMax(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public bool Equals(MinMax<T> other)
        {
            return Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        public override bool Equals(object? obj)
        {
            return obj is MinMax<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }

        public static bool operator ==(MinMax<T> left, MinMax<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MinMax<T> left, MinMax<T> right)
        {
            return !left.Equals(right);
        }
    }
}
