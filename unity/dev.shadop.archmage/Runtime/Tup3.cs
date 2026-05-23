#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// 0-based tuple with 3 elements. JSON serializes as {"item0": ..., "item1": ..., "item2": ...}.
    /// </summary>
    public class Tup3<T0, T1, T2>
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

        public override string ToString()
        {
            return $"({Item0}, {Item1}, {Item2})";
        }
    }
}
