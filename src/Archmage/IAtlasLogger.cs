namespace Shadop.Archmage;

/// <summary>
/// Interface for logging messages during Atlas loading.
/// </summary>
public interface IAtlasLogger
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void Info(string message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void Warn(string message);
}
