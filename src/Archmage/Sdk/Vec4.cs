#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Represents a 4D vector.
    /// </summary>
    public struct Vec4<T> : IEquatable<Vec4<T>> where T : unmanaged, IEquatable<T>
    {
        [JsonProperty("x")] public T X { get; set; }
        [JsonProperty("y")] public T Y { get; set; }
        [JsonProperty("z")] public T Z { get; set; }
        [JsonProperty("w")] public T W { get; set; }

        public Vec4(T x, T y, T z, T w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public bool Equals(Vec4<T> other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override bool Equals(object? obj)
        {
            return obj is Vec4<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }

        public static bool operator ==(Vec4<T> left, Vec4<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec4<T> left, Vec4<T> right)
        {
            return !left.Equals(right);
        }
    }
}
