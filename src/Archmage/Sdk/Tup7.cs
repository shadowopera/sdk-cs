#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// 0-based tuple with 7 elements. JSON serializes as {"item0": ..., ..., "item6": ...}.
    /// </summary>
    public class Tup7<T0, T1, T2, T3, T4, T5, T6>
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

        [JsonProperty("item5")]
        public T5 Item5 { get; set; }

        [JsonProperty("item6")]
        public T6 Item6 { get; set; }

        public Tup7(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            Item0 = item0;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
        }

        public object[] Values() => new object[] { Item0!, Item1!, Item2!, Item3!, Item4!, Item5!, Item6! };

        public void Deconstruct(out T0 item0, out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6)
        {
            item0 = Item0;
            item1 = Item1;
            item2 = Item2;
            item3 = Item3;
            item4 = Item4;
            item5 = Item5;
            item6 = Item6;
        }

        public override string ToString()
        {
            return $"({Item0}, {Item1}, {Item2}, {Item3}, {Item4}, {Item5}, {Item6})";
        }
    }
}
