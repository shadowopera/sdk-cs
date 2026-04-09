#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// 0-based tuple with 2 elements. JSON serializes as {"item0": ..., "item1": ...}.
    /// </summary>
    public class Tup2<T0, T1> : IEquatable<Tup2<T0, T1>>
    {
        [JsonProperty("item0")]
        public T0 Item0 { get; set; }

        [JsonProperty("item1")]
        public T1 Item1 { get; set; }

        public Tup2(T0 item0, T1 item1)
        {
            Item0 = item0;
            Item1 = item1;
        }

        public object[] Values() => new object[] { Item0!, Item1! };

        public void Deconstruct(out T0 item0, out T1 item1)
        {
            item0 = Item0;
            item1 = Item1;
        }

        public bool Equals(Tup2<T0, T1>? other)
        {
            if (other is null) return false;
            return EqualityComparer<T0>.Default.Equals(Item0, other.Item0) &&
                   EqualityComparer<T1>.Default.Equals(Item1, other.Item1);
        }

        public override bool Equals(object? obj)
        {
            return obj is Tup2<T0, T1> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item0, Item1);
        }

        public override string ToString()
        {
            return $"({Item0}, {Item1})";
        }

        public static bool operator ==(Tup2<T0, T1>? left, Tup2<T0, T1>? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Tup2<T0, T1>? left, Tup2<T0, T1>? right)
        {
            return !(left == right);
        }
    }
}
