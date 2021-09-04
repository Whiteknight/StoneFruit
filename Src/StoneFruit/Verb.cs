using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit
{
    /// <summary>
    /// A sequence of one or more words which is mapped to a handler.
    /// </summary>
    public struct Verb : IReadOnlyList<string>, IEquatable<Verb>
    {
        // Verb is just a wrapper around a List of string. The constructors assert that the
        // list is non-null, not empty, and that all the entries in the list are also non-null
        // and non-empty

        private readonly IReadOnlyList<string> _verb;

        public Verb(string verb)
        {
            if (string.IsNullOrEmpty(verb))
                throw new InvalidOperationException("Verb must contain at least one word");
            if (verb.Contains(' '))
                _verb = verb.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            else
                _verb = new[] { verb };
            if (_verb.Count == 0)
                throw new InvalidOperationException("Verb must contain at least one word");
        }

        public Verb(string[] verb)
        {
            if (verb == null || verb.Length == 0)
                throw new InvalidOperationException("Verb must contain at least one word");
            _verb = verb
                .SelectMany(w => (w ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
            if (_verb.Count == 0)
                throw new InvalidOperationException("Verb must contain at least one word");
        }

        public static implicit operator Verb(string s) => new Verb(s);

        public static implicit operator Verb(string[] s) => new Verb(s);

        public string this[int index] => _verb[index];

        public int Count => _verb.Count;

        public IEnumerator<string> GetEnumerator() => _verb.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)(_verb ?? Enumerable.Empty<string>())).GetEnumerator();

        public override string ToString() => string.Join(" ", _verb);

        public override int GetHashCode() => _verb.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is Verb asVerb)
                return Equals(asVerb);
            return false;
        }

        public bool Equals(Verb other)
        {
            if (Count != other.Count)
                return false;
            for (int i = 0; i < Count; i++)
            {
                if (this[i] != other[i])
                    return false;
            }
            return true;
        }

        public static bool operator ==(Verb a, Verb b) => a.Equals(b);

        public static bool operator !=(Verb a, Verb b) => !a.Equals(b);
    }
}
