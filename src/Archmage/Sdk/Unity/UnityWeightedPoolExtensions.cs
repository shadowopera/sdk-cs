#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Provides parameterless <c>Sample</c> and <c>SampleIndex</c> extension methods that draw an
    /// item from a <see cref="WeightedPool{T}"/> at random with probability proportional to its
    /// weight, using Unity's global <see cref="UnityEngine.Random"/>.
    /// </summary>
    public static class UnityWeightedPoolExtensions
    {
        // The total weight must not exceed this limit, keeping it within the int range.
        private const long _maxTotalWeight = 1_000_000_000;

        /// <summary>
        /// Returns a randomly selected item, weighted by the pool's weights.
        /// Throws if the pool is empty or the total weight is zero.
        /// </summary>
        public static T Sample<T>(this WeightedPool<T> wp)
        {
            return wp.Items![wp.SampleIndex()];
        }

        /// <summary>
        /// Returns the index of a randomly selected item, weighted by the pool's weights.
        /// Throws if the pool is empty or the total weight is zero.
        /// </summary>
        public static int SampleIndex<T>(this WeightedPool<T> wp)
        {
            if (wp.Items == null || wp.Items.Length == 0)
            {
                throw new ArchmageException("WeightedPool.SampleIndex: empty pool");
            }

            int[] weights = wp.Weights!;
            long total = 0;
            foreach (int w in weights)
            {
                total += w;
            }
            if (total == 0)
            {
                throw new ArchmageException("WeightedPool.SampleIndex: total weight is zero");
            }
            if (total > _maxTotalWeight)
            {
                throw new ArchmageException("WeightedPool.SampleIndex: total weight exceeds 1,000,000,000");
            }

            int value = Random.Range(0, (int)total);
            int acc = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                acc += weights[i];
                if (acc > value)
                {
                    return i;
                }
            }

            throw new ArchmageException("WeightedPool.SampleIndex: unreachable");
        }
    }
}

#endif
