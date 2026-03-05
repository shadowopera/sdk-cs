using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// 0-based tuple with 5 elements. JSON serializes as {"item0": ..., "item1": ..., "item2": ..., "item3": ..., "item4": ...}.
    /// </summary>
    public class Tup5<T0, T1, T2, T3, T4> : IEquatable<Tup5<T0, T1, T2, T3, T4>>
    {
        [JsonProperty("item0")]
        public T0 Item0 { get; set; }

        [JsonProperty("item1")]
        public T1 Item1 { get; set; }

        [JsonProperty("item2")]
        public T2 Item2 { get; set; }

        [JsonProperty("item3")]
        public T3 Item3 { get; set; }

        [JsonProperty("item4")]
        public T4 Item4 { get; set; }

        public Tup5(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item0 = item0;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public object[] Values() => new object[] { Item0!, Item1!, Item2!, Item3!, Item4! };

        public void Deconstruct(out T0 item0, out T1 item1, out T2 item2, out T3 item3, out T4 item4)
        {
            item0 = Item0;
            item1 = Item1;
            item2 = Item2;
            item3 = Item3;
            item4 = Item4;
        }

        public bool Equals(Tup5<T0, T1, T2, T3, T4>? other)
        {
            if (other is null) return false;
            return EqualityComparer<T0>.Default.Equals(Item0, other.Item0) &&
                   EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
                   EqualityComparer<T3>.Default.Equals(Item3, other.Item3) &&
                   EqualityComparer<T4>.Default.Equals(Item4, other.Item4);
        }

        public override bool Equals(object? obj)
        {
            return obj is Tup5<T0, T1, T2, T3, T4> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item0, Item1, Item2, Item3, Item4);
        }

        public override string ToString()
        {
            return $"({Item0}, {Item1}, {Item2}, {Item3}, {Item4})";
        }

        public static bool operator ==(Tup5<T0, T1, T2, T3, T4>? left, Tup5<T0, T1, T2, T3, T4>? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Tup5<T0, T1, T2, T3, T4>? left, Tup5<T0, T1, T2, T3, T4>? right)
        {
            return !(left == right);
        }
    }
}
