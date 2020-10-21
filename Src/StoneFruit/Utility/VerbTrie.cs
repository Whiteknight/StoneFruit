﻿using System;
using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Utility
{
    // Trie to lookup handlers or handler-related types using a set of positional arguments from
    // IArguments. Lookup tries to greedily consume as many positional arguments from the front
    // of the IArguments to get the most specific handler it can find.
    public class VerbTrie<TValue>
        where TValue : class
    {
        private readonly Node _root;

        public VerbTrie()
        {
            _root = new Node();
            Count = 0;
        }

        public void Insert(IEnumerable<string> keys, TValue value)
        {
            Assert.ArgumentNotNull(keys, nameof(keys));
            var current = _root;
            foreach (var key in keys)
                current = current.GetOrInsertChild(key);
            current.Value = value;
            Count++;
        }

        public int Count { get; private set; }

        //public bool Contains(IEnumerable<TKey> keys)
        //{
        //    return GetNode(keys) != null;
        //}

        public TValue Get(IArguments args)
        {
            // TODO V2: Should propagate this requirement further up the chain so people aren't
            // surprised to pass in a custom IArguments and not get what they expect
            var argsWithVerb = args as IVerbSource;
            if (argsWithVerb == null)
                return default;
            var node = GetNode(argsWithVerb);
            if (node == null)
                return default;
            return node.Value;
        }

        public IReadOnlyList<KeyValuePair<Verb, TValue>> GetAll()
        {
            var values = new List<KeyValuePair<Verb, TValue>>();
            _root.AppendAll("", values);
            return values;
        }

        public TValue Get(IEnumerable<string> keys)
        {
            var current = _root;
            foreach (var key in keys)
            {
                var node = current.GetChild(key);
                if (node == null)
                    return null;
                current = node;
            }
            return current.Value;
        }

        private Node GetNode(IVerbSource args)
        {
            var keys = args.GetVerbCandidatePositionals();
            if (keys.Count == 0)
                throw TrieException.NoArguments();

            var foundNodes = new List<(IPositionalArgument Arg, Node Node)>();

            // Loop over all the prospective initial positionals so long as we keep getting nodes.
            var current = _root;
            for (int index = 0; index < keys.Count; index++)
            {
                var arg = keys[index];
                var key = arg.AsString().ToLowerInvariant();
                var node = current.GetChild(key);
                if (node == null)
                    break;
                foundNodes.Add((arg, node));
                current = node;
            }

            // Start from the end of the list looping forwards. Keep looking until we find a
            // value. Once we find a value, mark all remaining args as consumed and return the
            // value
            Node targetNode = null;
            int foundIndex = -1;
            for (int index = foundNodes.Count - 1; index >= 0; index--)
            {
                if (targetNode != null)
                    continue;
                var node = foundNodes[index].Node;
                if (node.Value != null)
                {
                    targetNode = node;
                    foundIndex = index;
                }
            }
            if (foundIndex >= 0)
                args.SetVerbCount(foundIndex + 1);

            return targetNode;
        }

        private class Node
        {
            private readonly Dictionary<string, Node> _children;

            public Node()
            {
                _children = new Dictionary<string, Node>();
            }

            public TValue Value { get; set; }

            public Node GetChild(string key)
            {
                return _children.ContainsKey(key) ? _children[key] : null;
            }

            public Node GetOrInsertChild(string key)
            {
                if (!_children.ContainsKey(key))
                    _children.Add(key, new Node());
                return _children[key];
            }

            public void AppendAll(string current, List<KeyValuePair<Verb, TValue>> values)
            {
                if (Value != null)
                    values.Add(new KeyValuePair<Verb, TValue>(current, Value));
                foreach (var child in _children)
                {
                    var name = current + " " + child.Key;
                    child.Value.AppendAll(name, values);
                }
            }
        }
    }

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
