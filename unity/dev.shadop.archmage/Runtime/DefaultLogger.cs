using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Default console logger implementation that writes to standard error.
    /// </summary>
    internal class DefaultLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            Console.Error.WriteLine($"INF {message}");
        }
    }
}
