using Newtonsoft.Json;

namespace Shadop.Archmage;

/// <summary>
/// Represents a 3D vector. Serialized as JSON array [x, y, z].
/// </summary>
/// <typeparam name="T">The component type, must be equatable.</typeparam>
[JsonConverter(typeof(Vec3JsonConverter))]
public struct Vec3<T> : IEquatable<Vec3<T>>
    where T : IEquatable<T>
{
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }

    public Vec3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(Vec3<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) &&
               EqualityComparer<T>.Default.Equals(Y, other.Y) &&
               EqualityComparer<T>.Default.Equals(Z, other.Z);
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
