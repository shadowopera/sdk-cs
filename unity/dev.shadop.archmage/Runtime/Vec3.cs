#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Represents a 3D vector.
    /// </summary>
    public struct Vec3<T> : IEquatable<Vec3<T>> where T : unmanaged, IEquatable<T>
    {
        [JsonProperty("x")] public T X { get; set; }
        [JsonProperty("y")] public T Y { get; set; }
        [JsonProperty("z")] public T Z { get; set; }

        public Vec3(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Vec3<T> other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object? obj)
        {
            return obj is Vec3<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public static bool operator ==(Vec3<T> left, Vec3<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec3<T> left, Vec3<T> right)
        {
            return !left.Equals(right);
        }
    }
}
