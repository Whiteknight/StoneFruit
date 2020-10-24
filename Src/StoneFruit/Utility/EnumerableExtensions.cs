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
    }
}
