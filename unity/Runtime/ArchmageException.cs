namespace Shadop.Archmage;

/// <summary>
/// Exception thrown by the Archmage SDK.
/// </summary>
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
