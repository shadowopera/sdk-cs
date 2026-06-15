#nullable enable

using Newtonsoft.Json;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Holds items alongside their selection weights in two parallel arrays of equal length.
    /// The <c>Sample</c> and <c>SampleIndex</c> extension methods (in
    /// <see cref="WeightedPoolExtensions"/>) draw an item at random with probability
    /// proportional to its weight. The <c>SampleWithNoise</c> and <c>SampleIndexWithNoise</c>
    /// methods map a caller-supplied noise value across the cumulative weights instead.
    /// </summary>
    public class WeightedPool<T>
    {
        // The total weight must not exceed this limit, keeping it within the int range.
        private const long _maxTotalWeight = 1_000_000_000;

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

        /// <summary>
        /// Maps the <paramref name="noise"/> value (0 to 1) to an item according to the weights.
        /// <c>noise &lt; 0</c> returns the first item with non-zero weight; <c>noise &gt;= 1</c>
        /// returns the last. Throws if the pool is empty or the total weight is zero.
        /// </summary>
        public T SampleWithNoise(float noise)
        {
            return Items![SampleIndexWithNoise(noise)];
        }

        /// <summary>
        /// Maps the <paramref name="noise"/> value (0 to 1) to an item index according to the weights.
        /// <c>noise &lt; 0</c> returns the first index with non-zero weight; <c>noise &gt;= 1</c>
        /// returns the last. Throws if the pool is empty or the total weight is zero.
        /// </summary>
        public int SampleIndexWithNoise(float noise)
        {
            if (Items == null || Items.Length == 0)
            {
                throw new ArchmageException("WeightedPool.SampleIndexWithNoise: empty pool");
            }

            int[] weights = Weights!;
            long total = 0;
            foreach (int w in weights)
            {
                total += w;
            }
            if (total == 0)
            {
                throw new ArchmageException("WeightedPool.SampleIndexWithNoise: total weight is zero");
            }
            if (total > _maxTotalWeight)
            {
                throw new ArchmageException("WeightedPool.SampleIndexWithNoise: total weight exceeds 1,000,000,000");
            }

            int value = (int)((double)noise * total);
            if (value >= total)
            {
                value = (int)total - 1;
            }
            else if (value < 0)
            {
                value = 0;
            }

            int acc = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                acc += weights[i];
                if (acc > value)
                {
                    return i;
                }
            }

            throw new ArchmageException("WeightedPool.SampleIndexWithNoise: unreachable");
        }
    }
}
