#nullable enable

using System;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Represents a 2D vector.
    /// </summary>
    [JsonConverter(typeof(Vec2JsonConverter))]
    public struct Vec2<T> : IEquatable<Vec2<T>>
        where T : unmanaged, IEquatable<T>
    {
        public T X { get; set; }
        public T Y { get; set; }

        public Vec2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Vec2<T> other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Vec2<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(Vec2<T> left, Vec2<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec2<T> left, Vec2<T> right)
        {
            return !left.Equals(right);
        }
    }
}
