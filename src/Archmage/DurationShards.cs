namespace Shadop.Archmage
{
    /// <summary>
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Encodes Duration to compact array (chooses smallest representation based on value; zero → null).
        /// </summary>
        public static long[]? ShardDuration(Duration duration)
        {
            long ns = duration.Nanoseconds();
            if (ns == 0)
                return null;

            // Try encoding as seconds (no fractional part)
            if (ns % 1_000_000_000 == 0)
                return new[] { 0L, ns / 1_000_000_000 };

            // Try encoding as milliseconds
            if (ns % 1_000_000 == 0)
                return new[] { 1L, ns / 1_000_000 };

            // Try encoding as microseconds
            if (ns % 1_000 == 0)
                return new[] { 2L, ns / 1_000 };

            // Check if we can use mixed encoding (more compact than pure nanoseconds for large values)
            long seconds = ns / 1_000_000_000;
            long remainingNs = ns % 1_000_000_000;

            if (seconds != 0 && remainingNs != 0)
                return new[] { 4L, seconds, remainingNs };

            // Encode as nanoseconds
            return new[] { 3L, ns };
        }

        /// <summary>
        /// Decodes shard array to Duration (null/empty → Duration.Zero).
        /// </summary>
        /// <exception cref="ArchmageException">Thrown if invalid format or unknown type.</exception>
        public static Duration ParseDurationShards(long[]? shards)
        {
            if (shards == null || shards.Length == 0)
                return Duration.Zero;

            if (shards.Length < 2)
                throw new ArchmageException($"invalid duration shards length: {shards.Length}, expected at least 2");

            long type = shards[0];
            return type switch
            {
                0 => new Duration(shards[1] * 1_000_000_000), // seconds
                1 => new Duration(shards[1] * 1_000_000),     // milliseconds
                2 => new Duration(shards[1] * 1_000),         // microseconds
                3 => new Duration(shards[1]),                 // nanoseconds
                4 => ParseMixedShards(shards),                // mixed (seconds + nanoseconds)
                _ => throw new ArchmageException($"invalid duration shard type: {type}")
            };
        }

        static Duration ParseMixedShards(long[] shards)
        {
            if (shards.Length != 3)
                throw new ArchmageException($"mixed duration shards must have 3 elements, got {shards.Length}");

            long seconds = shards[1];
            long nanoseconds = shards[2];

            return new Duration(seconds * 1_000_000_000 + nanoseconds);
        }
    }
}
