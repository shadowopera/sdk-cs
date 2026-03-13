#nullable enable

#if UNITY_2017_1_OR_NEWER

namespace Shadop.Archmage
{
    /// <summary>
    /// Simple logger adapter to pipe Archmage internal output to Unity Console.
    /// </summary>
    public class UnityAtlasLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            UnityEngine.Debug.Log($"[Archmage] {message}");
        }
    }
}

#endif
