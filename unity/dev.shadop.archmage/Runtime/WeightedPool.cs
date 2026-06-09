#nullable enable

using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Holds items alongside their selection weights in two parallel arrays of equal length.
    /// The <c>Sample</c> and <c>SampleIndex</c> extension methods (in
    /// <see cref="WeightedPoolExtensions"/>) draw an item at random with probability
    /// proportional to its weight.
    /// </summary>
    public class WeightedPool<T>
    {
        /// <summary>The candidate values, one per weight.</summary>
        [JsonProperty("items")] public T[]? Items { get; set; }

        /// <summary>The non-negative selection weights, parallel to <see cref="Items"/>.</summary>
        [JsonProperty("weights")] public int[]? Weights { get; set; }

        public WeightedPool() { }

        public WeightedPool(T[] items, int[] weights)
        {
            Items = items;
            Weights = weights;
        }

        /// <summary>The number of items in the pool.</summary>
        [JsonIgnore] public int Count => Items?.Length ?? 0;
    }
}
