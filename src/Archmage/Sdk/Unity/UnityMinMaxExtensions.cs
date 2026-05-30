#nullable enable

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Provides parameterless <c>Sample</c> extension methods that draw a uniform random value
    /// from a <see cref="MinMax{T}"/> range using Unity's global <see cref="UnityEngine.Random"/>.
    /// The returned value lies in <c>[Min, Max]</c> (both ends included).
    /// </summary>
    public static class UnityMinMaxExtensions
    {
        public static sbyte Sample(this MinMax<sbyte> mm)
        {
            int span = mm.Max - mm.Min;
            return (sbyte)(mm.Min + Random.Range(0, span + 1));
        }

        public static short Sample(this MinMax<short> mm)
        {
            int span = mm.Max - mm.Min;
            return (short)(mm.Min + Random.Range(0, span + 1));
        }

        public static int Sample(this MinMax<int> mm)
        {
            int span = mm.Max - mm.Min;
            return mm.Min + Random.Range(0, span + 1);
        }

        public static long Sample(this MinMax<long> mm)
        {
            long span = mm.Max - mm.Min;
            return mm.Min + Random.Range(0, (int)(span + 1));
        }

        public static byte Sample(this MinMax<byte> mm)
        {
            int span = mm.Max - mm.Min;
            return (byte)(mm.Min + Random.Range(0, span + 1));
        }

        public static ushort Sample(this MinMax<ushort> mm)
        {
            int span = mm.Max - mm.Min;
            return (ushort)(mm.Min + Random.Range(0, span + 1));
        }

        public static uint Sample(this MinMax<uint> mm)
        {
            uint span = mm.Max - mm.Min;
            return mm.Min + (uint)Random.Range(0, (int)(span + 1));
        }

        public static ulong Sample(this MinMax<ulong> mm)
        {
            ulong span = mm.Max - mm.Min;
            return mm.Min + (ulong)Random.Range(0, (int)(span + 1));
        }

        public static float Sample(this MinMax<float> mm)
        {
            return mm.Min + Random.value * (mm.Max - mm.Min);
        }

        public static double Sample(this MinMax<double> mm)
        {
            return mm.Min + (double)Random.value * (mm.Max - mm.Min);
        }
    }
}

#endif
