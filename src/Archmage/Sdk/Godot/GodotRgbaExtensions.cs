#nullable enable

using Godot;

namespace Shadop.Archmage.Sdk
{
    public static class GodotRgbaExtensions
    {
        /// <summary>
        /// Converts an <see cref="Rgba"/> value to a <c>Godot.Color</c>.
        /// Each channel is mapped from [0, 255] to [0, 1].
        /// </summary>
        public static Color ToColor(this Rgba rgba)
            => Color.Color8(rgba.R, rgba.G, rgba.B, rgba.A);
    }
}
