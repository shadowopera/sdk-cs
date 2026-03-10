#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents a 2D vector. Serialized as JSON array [x, y].
    /// </summary>
    [JsonConverter(typeof(Vec2JsonConverter))]
    public struct Vec2<T> : IEquatable<Vec2<T>>
        where T : IEquatable<T>
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
            return EqualityComparer<T>.Default.Equals(X, other.X) &&
                   EqualityComparer<T>.Default.Equals(Y, other.Y);
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
