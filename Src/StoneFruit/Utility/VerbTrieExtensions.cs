using System;
using System.Collections.Generic;

namespace StoneFruit.Utility
{
    public static class VerbTrieExtensions
    {
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

        public static VerbTrie<T> ToVerbTrie<T>(this IEnumerable<T> source, Func<T, Verb> getVerb)
            where T : class
            => ToVerbTrie(source, getVerb, v => v);
    }
}
