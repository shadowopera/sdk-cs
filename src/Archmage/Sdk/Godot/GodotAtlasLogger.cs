#nullable enable

using Godot;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Simple logger adapter to pipe Archmage internal output to the Godot output with <c>GD.Print</c>, which can be
    /// called on any thread.
    /// </summary>
    public class GodotAtlasLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            GD.Print(message);
        }
    }
}
