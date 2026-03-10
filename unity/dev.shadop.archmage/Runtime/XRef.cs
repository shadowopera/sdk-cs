using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents a cross-table reference using a raw identifier and a resolved object reference.
    /// </summary>
    /// <remarks>
    /// <para>XRef is a two-part structure: it stores both the raw identifier (serialized to JSON)
    /// and the resolved reference (set during the binding phase). This enables lazy resolution of references
    /// after all data is loaded.</para>
    /// <para><strong>Important:</strong> V should be int, long, or string (the identifier type).
    /// Using other types may cause issues during serialization or reference binding.</para>
    /// </remarks>
    [JsonConverter(typeof(XRefJsonConverter))]
    public struct XRef<V, T>
        where V : notnull
        where T : class
    {
        /// <summary>
        /// Raw identifier serialized to JSON; key for lookup during binding phase.
        /// </summary>
        public V RawValue { get; set; }

        /// <summary>
        /// Resolved object (not serialized). Populated by Atlas.BindRefs(); may be null if unresolved.
        /// </summary>
        [JsonIgnore]
        public T? Ref { get; set; }

        public XRef(V rawValue, T? refValue)
        {
            RawValue = rawValue;
            Ref = refValue;
        }
    }
}
