using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;

namespace StoneFruit;

/// <summary>
/// A sequence of one or more words which is mapped to a handler.
/// </summary>
public readonly struct Verb : IReadOnlyList<string>, IEquatable<Verb>
{
    private const string _errorNoWords = "Verb must contain at least one word";
    // Verb is just a wrapper around a List of string. The constructors assert that the
    // list is non-null, not empty, and that all the entries in the list are also non-null
    // and non-empty

    private readonly string[] _verb;

    public Verb(string verb)
    {
        if (string.IsNullOrEmpty(verb))
            throw new InvalidOperationException(_errorNoWords);
        _verb = verb.Contains(' ')
            ? verb.Split(Constants.SeparatedBySpace, StringSplitOptions.RemoveEmptyEntries)
            : [verb];
        if (_verb.Length == 0)
            throw new InvalidOperationException(_errorNoWords);
    }

    public Verb(string[] verb)
    {
        if (verb == null || verb.Length == 0)
            throw new InvalidOperationException(_errorNoWords);
        _verb = verb
            .SelectMany(w => (w ?? "").Split(Constants.SeparatedBySpace, StringSplitOptions.RemoveEmptyEntries))
            .ToArray();
        if (_verb.Length == 0)
            throw new InvalidOperationException(_errorNoWords);
    }

    public static implicit operator Verb(string s) => new Verb(s);

    public static implicit operator Verb(string[] s) => new Verb(s);

    public string this[int index] => _verb[index];

    public int Count => _verb.Length;

    public IEnumerator<string> GetEnumerator() => ((IReadOnlyList<string>)_verb).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _verb.GetEnumerator();

    public override string ToString() => string.Join(" ", _verb);

    public override int GetHashCode() => _verb.GetHashCode();

    public override bool Equals(object? obj)
        => obj is Verb asVerb && Equals(asVerb);

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
