using Newtonsoft.Json;

namespace Shadop.Archmage;

/// <summary>
/// 0-based tuple with 1 element. JSON serializes as {"item0": ...}.
/// </summary>
public struct Tup1<T0> : IEquatable<Tup1<T0>>
{
    [JsonProperty("item0")]
    public T0 Item0 { get; set; }

    public Tup1(T0 item0)
    {
        Item0 = item0;
    }

    public object[] Values() => [Item0!];

    public void Deconstruct(out T0 item0)
    {
        item0 = Item0;
    }

    public bool Equals(Tup1<T0> other)
    {
        return EqualityComparer<T0>.Default.Equals(Item0, other.Item0);
    }

    public override bool Equals(object? obj)
    {
        return obj is Tup1<T0> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T0>.Default.GetHashCode(Item0!);
    }

    public override string ToString()
    {
        return $"({Item0})";
    }

    public static bool operator ==(Tup1<T0> left, Tup1<T0> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Tup1<T0> left, Tup1<T0> right)
    {
        return !left.Equals(right);
    }
}
