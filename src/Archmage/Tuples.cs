#nullable enable

namespace Shadop.Archmage
{
    /// <summary>
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        public static Tup1<T0> MakeTuple<T0>(T0 item0)
        {
            return new Tup1<T0>(item0);
        }

        public static Tup2<T0, T1> MakeTuple<T0, T1>(T0 item0, T1 item1)
        {
            return new Tup2<T0, T1>(item0, item1);
        }

        public static Tup3<T0, T1, T2> MakeTuple<T0, T1, T2>(T0 item0, T1 item1, T2 item2)
        {
            return new Tup3<T0, T1, T2>(item0, item1, item2);
        }

        public static Tup4<T0, T1, T2, T3> MakeTuple<T0, T1, T2, T3>(T0 item0, T1 item1, T2 item2, T3 item3)
        {
            return new Tup4<T0, T1, T2, T3>(item0, item1, item2, item3);
        }

        public static Tup5<T0, T1, T2, T3, T4> MakeTuple<T0, T1, T2, T3, T4>(
            T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Tup5<T0, T1, T2, T3, T4>(item0, item1, item2, item3, item4);
        }

        public static Tup6<T0, T1, T2, T3, T4, T5> MakeTuple<T0, T1, T2, T3, T4, T5>(
            T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new Tup6<T0, T1, T2, T3, T4, T5>(item0, item1, item2, item3, item4, item5);
        }

        public static Tup7<T0, T1, T2, T3, T4, T5, T6> MakeTuple<T0, T1, T2, T3, T4, T5, T6>(
            T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            return new Tup7<T0, T1, T2, T3, T4, T5, T6>(item0, item1, item2, item3, item4, item5, item6);
        }
    }
}
