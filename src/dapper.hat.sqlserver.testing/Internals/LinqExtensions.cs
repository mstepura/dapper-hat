using System;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.Hat.SqlServer.Testing
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// Full outer join of two sets: "A" and "B".
        /// </summary>
        /// <example>
        /// {1,2,3,3}.FullOuterJoin({2,3,4})
        /// results in:
        /// |    1 | null |
        /// |    2 |    2 |
        /// |    3 |    3 |
        /// |    3 |    3 |
        /// | null |    4 |
        /// </example>
        /// <typeparam name="TA">Type of "A" set elements</typeparam>
        /// <typeparam name="TB">Type of "B" set elements</typeparam>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="setA">"A" set is evaluated lazily</param>
        /// <param name="setB">"B" set is pre-fetched into Lookup instance</param>
        /// <param name="keySelectorA">Key selector for TA elements</param>
        /// <param name="keySelectorB">Key selector for TB elements</param>
        /// <param name="resultSelector">Function to construct result type from TA and TB</param>
        /// <param name="keyEqualityComparer">Equality comparer for TKey type. Default comparer is used if this parameter is null.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> FullOuterJoin<TA, TB, TKey, TResult>(
            this IEnumerable<TA> setA,
            IEnumerable<TB> setB,
            Func<TA, TKey> keySelectorA,
            Func<TB, TKey> keySelectorB,
            Func<TA, TB, TResult> resultSelector,
            IEqualityComparer<TKey> keyEqualityComparer = null)
        {
            if (keyEqualityComparer == null)
                keyEqualityComparer = EqualityComparer<TKey>.Default;

            var lookupB = setB.ToLookup(keySelectorB, keyEqualityComparer);
            var usedKeysB = new HashSet<TKey>(keyEqualityComparer);

            // output all items present in A
            foreach (var aItem in setA)
            {
                var key = keySelectorA(aItem);
                usedKeysB.Add(key);

                var bItems = lookupB[key];
                using (var bEnumerator = bItems.GetEnumerator())
                {
                    if (bEnumerator.MoveNext())
                    {
                        do
                        {
                            yield return resultSelector(aItem, bEnumerator.Current);
                        } while (bEnumerator.MoveNext());
                    }
                    else
                    {
                        yield return resultSelector(aItem, default(TB));
                    }
                }
            }

            // output all items present in B but not in A
            foreach (var bItem in lookupB
                .Where(bGroup => !usedKeysB.Contains(bGroup.Key))
                .SelectMany(bGroup => bGroup)
                )
            {
                yield return resultSelector(default(TA), bItem);
            }
        }
    }
}
