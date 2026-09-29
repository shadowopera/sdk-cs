#nullable enable

using Godot;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Simple logger adapter to pipe Archmage internal output to the Godot output with <c>GD.Print</c>.
    /// </summary>
    public class GodotAtlasLogger : IAtlasLogger
    {
        public void Info(string message)
        {
            GD.Print(message);
        }
    }
}
