using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Utility
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// If the source is null, returns an empty enumerable instead
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
            => source ?? Enumerable.Empty<T>();

        /// <summary>
        /// ToDictionary variant without key conflict errors. If the same key is encountered more than once,
        /// subsequent values are simply ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="getKey"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionaryUnique<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> getKey, Func<T, TValue> getValue)
        {
            Assert.ArgumentNotNull(source, nameof(source));
            Assert.ArgumentNotNull(getKey, nameof(getKey));
            Assert.ArgumentNotNull(getValue, nameof(getValue));

            var dict = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                var key = getKey(item);
                if (dict.ContainsKey(key))
                    continue;
                var value = getValue(item);
                dict.Add(key, value);
            }

            return dict;
        }

        /// <summary>
        /// ToDictionary variant without key conflict errors. If the same key is encountered more than once,
        /// subsequent values are simply ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static Dictionary<TKey, T> ToDictionaryUnique<T, TKey>(this IEnumerable<T> source, Func<T, TKey> getKey)
            => ToDictionaryUnique(source, getKey, t => t);

        /// <summary>
        /// ToDictionary variant without key conflict errors. If the same key is encountered more than once,
        /// subsequent values are simply ignored.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionaryUnique<TKey, TValue>(this IEnumerable<(TKey, TValue)> source)
            => ToDictionaryUnique(source, tuple => tuple.Item1, tuple => tuple.Item2);
    }
}
