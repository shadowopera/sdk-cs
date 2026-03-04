using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Default console logger implementation.
    /// </summary>
    internal class DefaultLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"INF {message}");
        }

        public void Warn(string message)
        {
            Console.WriteLine($"WRN {message}");
        }
    }
}
