#nullable enable

using System;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Default console logger implementation that writes to standard error.
    /// </summary>
    internal class DefaultLogger : IAtlasLogger
    {
        public void Info(string message)
        {
#if ARCHMAGE_DISABLE_DEFAULT_LOGGER || UNITY_5_3_OR_NEWER
            _ = message;
#else
            Console.Error.WriteLine($"INF {message}");
#endif
        }
    }
}
