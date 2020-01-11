using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
            => source ?? Enumerable.Empty<T>();

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

        public static Dictionary<TKey, T> ToDictionaryUnique<T, TKey>(this IEnumerable<T> source, Func<T, TKey> getKey)
            => ToDictionaryUnique(source, getKey, t => t);

        public static Dictionary<TKey, TValue> ToDictionaryUnique<TKey, TValue>(this IEnumerable<(TKey, TValue)> source)
            => ToDictionaryUnique(source, tuple => tuple.Item1, tuple => tuple.Item2);
    }
}
