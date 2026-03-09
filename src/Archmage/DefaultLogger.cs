using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Default console logger implementation that writes to standard output.
    /// </summary>
    internal class DefaultLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"INF {message}");
        }
    }
}
