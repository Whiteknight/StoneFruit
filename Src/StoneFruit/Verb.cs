using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit
{
    public struct Verb : IReadOnlyList<string>, IEquatable<Verb>
    {
        private readonly IReadOnlyList<string> _verb;

        public Verb(string verb)
        {
            if (verb.Contains(' '))
                _verb = verb.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            else
                _verb = new[] { verb };
        }

        public Verb(string[] verb)
        {
            if (verb.Length == 1 && verb[0].Contains(' '))
                _verb = verb[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            else
                _verb = verb;
        }

        public static implicit operator Verb(string s) => new Verb(s);

        public static implicit operator Verb(string[] s) => new Verb(s);

        public string this[int index] => _verb[index];

        public int Count => _verb.Count;

        public IEnumerator<string> GetEnumerator() => (_verb ?? Enumerable.Empty<string>()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)(_verb ?? Enumerable.Empty<string>())).GetEnumerator();

        public override string ToString() => string.Join(" ", _verb);

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
    }
}
