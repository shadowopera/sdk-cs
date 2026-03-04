using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Stores a raw value and a resolved reference.
    /// Only RawValue is serialized to JSON.
    /// <para>
    /// Note: TKey should be either int, long or string. Using other types is not supported
    /// and may cause issues during JSON serialization or reference binding.
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">The key type (should be int, long or string).</typeparam>
    /// <typeparam name="TValue">The reference value type.</typeparam>
    [JsonConverter(typeof(RefJsonConverter))]
    public class Ref<TKey, TValue>
        where TKey : notnull
        where TValue : class
    {
        /// <summary>
        /// The raw key value that will be serialized to JSON.
        /// </summary>
        public TKey RawValue { get; set; } = default!;

        /// <summary>
        /// The resolved reference. This is not serialized and should be set
        /// during the reference binding phase.
        /// </summary>
        [JsonIgnore]
        public TValue? Value { get; set; }

        public Ref()
        {
        }

        public Ref(TKey rawValue)
        {
            RawValue = rawValue;
        }
    }
}
