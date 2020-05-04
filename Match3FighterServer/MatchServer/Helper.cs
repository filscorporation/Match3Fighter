using System;
using System.Collections.Generic;

namespace MatchServer
{
    public static class Helper
    {
        /// <summary>
        /// Randomly shuffles list of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
