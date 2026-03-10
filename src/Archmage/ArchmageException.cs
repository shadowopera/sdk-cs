#nullable enable

using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Exception thrown by the Archmage SDK during configuration loading and processing.
    /// </summary>
    /// <remarks>
    /// All exceptions raised by the SDK use this exception type and automatically include the "&lt;archmage&gt;"
    /// prefix in the message for easy identification.
    /// </remarks>
    public class ArchmageException : Exception
    {
        public ArchmageException(string message) : base($"<archmage> {message}")
        {
        }

        public ArchmageException(string message, Exception innerException)
            : base($"<archmage> {message}", innerException)
        {
        }
    }
}
