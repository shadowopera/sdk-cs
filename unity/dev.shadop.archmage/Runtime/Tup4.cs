#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// 0-based tuple with 4 elements. JSON serializes as {"item0": ..., "item1": ..., "item2": ..., "item3": ...}.
    /// </summary>
    public class Tup4<T0, T1, T2, T3> : IEquatable<Tup4<T0, T1, T2, T3>>
    {
        [JsonProperty("item0")]
        public T0 Item0 { get; set; }

        [JsonProperty("item1")]
        public T1 Item1 { get; set; }

        [JsonProperty("item2")]
        public T2 Item2 { get; set; }

        [JsonProperty("item3")]
        public T3 Item3 { get; set; }

        public Tup4(T0 item0, T1 item1, T2 item2, T3 item3)
        {
            Item0 = item0;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public object[] Values() => new object[] { Item0!, Item1!, Item2!, Item3! };

        public void Deconstruct(out T0 item0, out T1 item1, out T2 item2, out T3 item3)
        {
            item0 = Item0;
            item1 = Item1;
            item2 = Item2;
            item3 = Item3;
        }

        public bool Equals(Tup4<T0, T1, T2, T3>? other)
        {
            if (other is null) return false;
            return EqualityComparer<T0>.Default.Equals(Item0, other.Item0) &&
                   EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
                   EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
        }

        public override bool Equals(object? obj)
        {
            return obj is Tup4<T0, T1, T2, T3> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item0, Item1, Item2, Item3);
        }

        public override string ToString()
        {
            return $"({Item0}, {Item1}, {Item2}, {Item3})";
        }

        public static bool operator ==(Tup4<T0, T1, T2, T3>? left, Tup4<T0, T1, T2, T3>? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Tup4<T0, T1, T2, T3>? left, Tup4<T0, T1, T2, T3>? right)
        {
            return !(left == right);
        }
    }
}
