#nullable enable

using System;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Provides <c>Sample</c> extension methods that draw a uniform random value from a
    /// <see cref="MinMax{T}"/> range using a <see cref="System.Random"/> source. The returned
    /// value lies in <c>[Min, Max]</c> (both ends included).
    /// </summary>
    public static class MinMaxExtensions
    {
        // Distinct outcomes of a single closed-unit draw: 2^24 + 1, spanning [0, 2^24].
        private const int _closedUnitOutcomes = (1 << 24) + 1;

        // 1 / 2^24
        private const float _inverseFloatSignificand = 1.0f / (1 << 24);
        private const double _inverseDoubleSignificand = 1.0 / (1 << 24);

        public static sbyte Sample(this MinMax<sbyte> mm, Random rng)
        {
            int span = mm.Max - mm.Min;
            return (sbyte)(mm.Min + rng.Next(0, span + 1));
        }

        public static short Sample(this MinMax<short> mm, Random rng)
        {
            int span = mm.Max - mm.Min;
            return (short)(mm.Min + rng.Next(0, span + 1));
        }

        public static int Sample(this MinMax<int> mm, Random rng)
        {
            int span = mm.Max - mm.Min;
            return mm.Min + rng.Next(0, span + 1);
        }

        public static long Sample(this MinMax<long> mm, Random rng)
        {
            long span = mm.Max - mm.Min;
            return mm.Min + rng.Next(0, (int)(span + 1));
        }

        public static byte Sample(this MinMax<byte> mm, Random rng)
        {
            int span = mm.Max - mm.Min;
            return (byte)(mm.Min + rng.Next(0, span + 1));
        }

        public static ushort Sample(this MinMax<ushort> mm, Random rng)
        {
            int span = mm.Max - mm.Min;
            return (ushort)(mm.Min + rng.Next(0, span + 1));
        }

        public static uint Sample(this MinMax<uint> mm, Random rng)
        {
            uint span = mm.Max - mm.Min;
            return mm.Min + (uint)rng.Next(0, (int)(span + 1));
        }

        public static ulong Sample(this MinMax<ulong> mm, Random rng)
        {
            ulong span = mm.Max - mm.Min;
            return mm.Min + (ulong)rng.Next(0, (int)(span + 1));
        }

        public static float Sample(this MinMax<float> mm, Random rng)
        {
            float r = rng.Next(_closedUnitOutcomes) * _inverseFloatSignificand;
            return mm.Min + r * (mm.Max - mm.Min);
        }

        public static double Sample(this MinMax<double> mm, Random rng)
        {
            double r = rng.Next(_closedUnitOutcomes) * _inverseDoubleSignificand;
            return mm.Min + r * (mm.Max - mm.Min);
        }
    }
}
