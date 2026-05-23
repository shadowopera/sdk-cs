#nullable enable

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// 0-based tuple with 1 element. JSON serializes as {"item0": ...}.
    /// </summary>
    public class Tup1<T0>
    {
        [JsonProperty("item0")]
        public T0 Item0 { get; set; }

        public Tup1(T0 item0)
        {
            Item0 = item0;
        }

        public object[] Values() => new object[] { Item0! };

        public void Deconstruct(out T0 item0)
        {
            item0 = Item0;
        }

        public override string ToString()
        {
            return $"({Item0})";
        }
    }
}
