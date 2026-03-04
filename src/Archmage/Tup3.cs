using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// 0-based tuple with 3 elements. JSON serializes as {"item0": ..., "item1": ..., "item2": ...}.
    /// </summary>
    public struct Tup3<T0, T1, T2> : IEquatable<Tup3<T0, T1, T2>>
    {
        [JsonProperty("item0")]
        public T0 Item0 { get; set; }

        [JsonProperty("item1")]
        public T1 Item1 { get; set; }

        [JsonProperty("item2")]
        public T2 Item2 { get; set; }

        public Tup3(T0 item0, T1 item1, T2 item2)
        {
            Item0 = item0;
            Item1 = item1;
            Item2 = item2;
        }

        public object[] Values() => new object[] { Item0!, Item1!, Item2! };

        public void Deconstruct(out T0 item0, out T1 item1, out T2 item2)
        {
            item0 = Item0;
            item1 = Item1;
            item2 = Item2;
        }

        public bool Equals(Tup3<T0, T1, T2> other)
        {
            return EqualityComparer<T0>.Default.Equals(Item0, other.Item0) &&
                   EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        public override bool Equals(object? obj)
        {
            return obj is Tup3<T0, T1, T2> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item0, Item1, Item2);
        }

        public override string ToString()
        {
            return $"({Item0}, {Item1}, {Item2})";
        }

        public static bool operator ==(Tup3<T0, T1, T2> left, Tup3<T0, T1, T2> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tup3<T0, T1, T2> left, Tup3<T0, T1, T2> right)
        {
            return !left.Equals(right);
        }
    }
}
