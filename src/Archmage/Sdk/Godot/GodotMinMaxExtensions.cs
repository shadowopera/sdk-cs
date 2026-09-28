#nullable enable

using Godot;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Provides parameterless <c>Sample</c> extension methods that draw a uniform random value
    /// from a <see cref="MinMax{T}"/> range using Godot's global random number generator, which <c>GD.Seed</c>
    /// affects. The returned value lies in <c>[Min, Max]</c> (both ends included).
    /// </summary>
    public static class GodotMinMaxExtensions
    {
        public static sbyte Sample(this MinMax<sbyte> mm)
        {
            int span = mm.Max - mm.Min;
            return (sbyte)(mm.Min + GD.RandRange(0, span));
        }

        public static short Sample(this MinMax<short> mm)
        {
            int span = mm.Max - mm.Min;
            return (short)(mm.Min + GD.RandRange(0, span));
        }

        public static int Sample(this MinMax<int> mm)
        {
            int span = mm.Max - mm.Min;
            return mm.Min + GD.RandRange(0, span);
        }

        public static long Sample(this MinMax<long> mm)
        {
            long span = mm.Max - mm.Min;
            return mm.Min + GD.RandRange(0, (int)span);
        }

        public static byte Sample(this MinMax<byte> mm)
        {
            int span = mm.Max - mm.Min;
            return (byte)(mm.Min + GD.RandRange(0, span));
        }

        public static ushort Sample(this MinMax<ushort> mm)
        {
            int span = mm.Max - mm.Min;
            return (ushort)(mm.Min + GD.RandRange(0, span));
        }

        public static uint Sample(this MinMax<uint> mm)
        {
            uint span = mm.Max - mm.Min;
            return mm.Min + (uint)GD.RandRange(0, (int)span);
        }

        public static ulong Sample(this MinMax<ulong> mm)
        {
            ulong span = mm.Max - mm.Min;
            return mm.Min + (ulong)GD.RandRange(0, (int)span);
        }

        public static float Sample(this MinMax<float> mm)
        {
            return mm.Min + GD.Randf() * (mm.Max - mm.Min);
        }

        public static double Sample(this MinMax<double> mm)
        {
            return mm.Min + (double)GD.Randf() * (mm.Max - mm.Min);
        }

        public static Duration Sample(this MinMax<Duration> mm)
        {
            long min = mm.Min.Milliseconds();
            long max = mm.Max.Milliseconds();
            return new Duration((min + GD.RandRange(0, (int)(max - min))) * 1_000_000);
        }
    }
}
