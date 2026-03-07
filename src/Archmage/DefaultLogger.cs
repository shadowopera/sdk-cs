using System;

namespace Shadop.Archmage
{
    /// <summary>
    /// Default console logger implementation.
    /// </summary>
    class DefaultLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"INF {message}");
        }
    }
}
