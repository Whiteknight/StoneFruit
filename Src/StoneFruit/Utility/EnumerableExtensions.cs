using System;
using System.Collections.Generic;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Utility;

public static class EnumerableExtensions
{
    /// <summary>
    /// If the source is null, returns an empty enumerable instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        => source ?? [];

    /// <summary>
    /// During enumeration, execute a callback on every item without returning a different
    /// item or modifying the enumeration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="onEach"></param>
    /// <returns></returns>
    public static IEnumerable<T> Tap<T>(this IEnumerable<T> source, Action<T> onEach)
    {
        NotNull(onEach);

        foreach (var item in NotNull(source))
        {
            onEach(item);
            yield return item;
        }
    }
}
