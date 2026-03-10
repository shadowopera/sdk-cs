using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents a 4D vector. Serialized as JSON array [x, y, z, w].
    /// </summary>
    [JsonConverter(typeof(Vec4JsonConverter))]
    public struct Vec4<T> : IEquatable<Vec4<T>>
        where T : IEquatable<T>
    {
        public T X { get; set; }

        public T Y { get; set; }

        public T Z { get; set; }

        public T W { get; set; }

        public Vec4(T x, T y, T z, T w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public bool Equals(Vec4<T> other)
        {
            return EqualityComparer<T>.Default.Equals(X, other.X) &&
                   EqualityComparer<T>.Default.Equals(Y, other.Y) &&
                   EqualityComparer<T>.Default.Equals(Z, other.Z) &&
                   EqualityComparer<T>.Default.Equals(W, other.W);
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
