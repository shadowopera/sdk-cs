#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// 0-based tuple with 2 elements. JSON serializes as {"item0": ..., "item1": ...}.
    /// </summary>
    public class Tup2<T0, T1>
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

        public override string ToString()
        {
            return $"({Item0}, {Item1})";
        }
    }
}
