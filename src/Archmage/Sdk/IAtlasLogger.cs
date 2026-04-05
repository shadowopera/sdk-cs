#nullable enable

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Interface for logging during Atlas loading and operations.
    /// </summary>
    /// <remarks>
    /// Implement this interface to provide custom logging behavior for the Atlas loading system.
    /// The default implementation logs to the console with standardized prefixes.
    /// </remarks>
    public interface IAtlasLogger
    {
        void Info(string message);
    }
}
