#nullable enable

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// A logger that silently discards all log messages.
    /// </summary>
    public class NullLogger : IAtlasLogger
    {
        public void Info(string message)
        {
        }
    }
}
