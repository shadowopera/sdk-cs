namespace Shadop.Archmage
{
    /// <summary>
    /// Provides utility functions.
    /// </summary>
    public static partial class Archmage
    {
        /// <summary>
        /// Creates a 1-element tuple.
        /// </summary>
        public static Tup1<T0> Make<T0>(T0 item0)
        {
            return new Tup1<T0>(item0);
        }

        /// <summary>
        /// Creates a 2-element tuple.
        /// </summary>
        public static Tup2<T0, T1> Make<T0, T1>(T0 item0, T1 item1)
        {
            return new Tup2<T0, T1>(item0, item1);
        }

        /// <summary>
        /// Creates a 3-element tuple.
        /// </summary>
        public static Tup3<T0, T1, T2> Make<T0, T1, T2>(T0 item0, T1 item1, T2 item2)
        {
            return new Tup3<T0, T1, T2>(item0, item1, item2);
        }

        /// <summary>
        /// Creates a 4-element tuple.
        /// </summary>
        public static Tup4<T0, T1, T2, T3> Make<T0, T1, T2, T3>(T0 item0, T1 item1, T2 item2, T3 item3)
        {
            return new Tup4<T0, T1, T2, T3>(item0, item1, item2, item3);
        }

        /// <summary>
        /// Creates a 5-element tuple.
        /// </summary>
        public static Tup5<T0, T1, T2, T3, T4> Make<T0, T1, T2, T3, T4>(
            T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Tup5<T0, T1, T2, T3, T4>(item0, item1, item2, item3, item4);
        }

        /// <summary>
        /// Creates a 6-element tuple.
        /// </summary>
        public static Tup6<T0, T1, T2, T3, T4, T5> Make<T0, T1, T2, T3, T4, T5>(
            T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new Tup6<T0, T1, T2, T3, T4, T5>(item0, item1, item2, item3, item4, item5);
        }

        /// <summary>
        /// Creates a 7-element tuple.
        /// </summary>
        public static Tup7<T0, T1, T2, T3, T4, T5, T6> Make<T0, T1, T2, T3, T4, T5, T6>(
            T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            return new Tup7<T0, T1, T2, T3, T4, T5, T6>(item0, item1, item2, item3, item4, item5, item6);
        }
    }
}
