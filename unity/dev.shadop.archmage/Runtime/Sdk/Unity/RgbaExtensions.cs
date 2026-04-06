#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Shadop.Archmage.Sdk
{
    public static class RgbaExtensions
    {
        /// <summary>
        /// Converts an <see cref="Rgba"/> value to a <c>UnityEngine.Color</c>.
        /// Each channel is mapped from [0, 255] to [0, 1].
        /// </summary>
        public static Color ToColor(this Rgba rgba)
            => new Color(rgba.R / 255f, rgba.G / 255f, rgba.B / 255f, rgba.A / 255f);
    }
}

#endif
