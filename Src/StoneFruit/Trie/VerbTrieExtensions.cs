using System;
using System.Collections.Generic;

namespace StoneFruit.Trie;

public static class VerbTrieExtensions
{
    /// <summary>
    /// Convert an enumeration into a VerbTrie.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="getVerb"></param>
    /// <param name="getValue"></param>
    /// <returns></returns>
    public static VerbTrie<TValue> ToVerbTrie<T, TValue>(this IEnumerable<T> source, Func<T, Verb> getVerb, Func<T, TValue> getValue)
        where TValue : class
    {
        var trie = new VerbTrie<TValue>();
        foreach (var s in source)
        {
            var verb = getVerb(s);
            var value = getValue(s);
            trie.Insert(verb, value);
        }

        return trie;
    }

    /// <summary>
    /// Convert an enumeration into a VerbTrie.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="getVerb"></param>
    /// <returns></returns>
    public static VerbTrie<T> ToVerbTrie<T>(this IEnumerable<T> source, Func<T, Verb> getVerb)
        where T : class
        => ToVerbTrie(source, getVerb, v => v);
}
