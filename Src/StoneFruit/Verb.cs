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
            // TODO V2: If verb contains spaces, split it
            _verb = new[] { verb };
        }

        public Verb(string[] verb)
        {
            // TODO V2: If verb has Length==1 and verb[0] contains spaces, split it
            _verb = verb;
        }

        public static implicit operator Verb(string s) => new Verb(s);
        public static implicit operator Verb(string[] s) => new Verb(s);

        public string this[int index] => _verb == null ? null : _verb[index];

        public int Count => _verb == null ? 0 : _verb.Count;

        public IEnumerator<string> GetEnumerator() => (_verb ?? Enumerable.Empty<string>()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)(_verb ?? Enumerable.Empty<string>())).GetEnumerator();

        public override string ToString() => string.Join(" ", _verb);

        public bool Equals(Verb other)
        {
            if (this.Count != other.Count)
                return false;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != other[i])
                    return false;
            }
            return true;
        }
    }
}
