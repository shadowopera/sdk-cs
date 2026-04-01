#nullable enable

using Newtonsoft.Json;

namespace Shadop.Archmage
{
    /// <summary>
    /// Represents a cross-table reference using a config ID and a resolved object reference.
    /// </summary>
    /// <remarks>
    /// <para>XRef is a two-part structure: it stores both the config ID (serialized to JSON)
    /// and the resolved reference (set during the binding phase). This enables lazy resolution of references
    /// after all data is loaded.</para>
    /// </remarks>
    [JsonConverter(typeof(XRefJsonConverter))]
    public struct XRef<V, T>
        where V : notnull
        where T : class
    {
        /// <summary>
        /// Config ID serialized to JSON; key for lookup during binding phase.
        /// </summary>
        public V CfgId { get; set; }

        /// <summary>
        /// Resolved object (not serialized). Populated by Atlas.BindRefs(); may be null if unresolved.
        /// </summary>
        [JsonIgnore]
        public T? Ref { get; set; }

        public XRef(V cfgId, T? refValue)
        {
            CfgId = cfgId;
            Ref = refValue;
        }
    }
}
