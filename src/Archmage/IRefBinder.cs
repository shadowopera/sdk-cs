namespace Shadop.Archmage
{
    /// <summary>
    /// Interface for types that need to bind references after deserialization.
    /// Typically implemented by configuration table classes to resolve Ref fields.
    /// </summary>
    public interface IRefBinder
    {
        /// <summary>
        /// Binds references in this object. The atlas parameter provides access
        /// to all loaded configuration tables for reference resolution.
        /// </summary>
        /// <param name="atlas">The atlas instance containing all configuration tables.</param>
        void BindRefs(object atlas);
    }
}
