using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// A sequence of one or more words which is mapped to a handler.
/// </summary>
public readonly struct Verb : IReadOnlyList<string>, IEquatable<Verb>
{
    private const string _errorNoWords = "Verb must contain at least one word";
    private const string _errorInvalidHandler = "Object is not a valid handler type and a verb cannot be determined";
    // Verb is just a wrapper around a List of string. The constructors assert that the
    // list is non-null, not empty, and that all the entries in the list are also non-null
    // and non-empty

    private readonly string[] _verb;

    private Verb(string[] verb)
    {
        _verb = NotNullOrEmpty(verb.Where(v => !string.IsNullOrEmpty(v)).ToArray());
    }

    public static Result<Verb, Error> TryCreate(string verb)
    {
        if (string.IsNullOrEmpty(verb))
            return new NoWords();
        var words = verb.Contains(' ')
            ? verb.Split(Constants.SeparatedBySpace, StringSplitOptions.RemoveEmptyEntries)
            : [verb];
        if (words.Length == 0)
            return new NoWords();
        return new Verb(words);
    }

    public static Result<Verb, Error> TryCreate(string[] verb)
    {
        var words = verb
            .OrEmptyIfNull()
            .SelectMany(w => (w ?? "").Split(Constants.SeparatedBySpace, StringSplitOptions.RemoveEmptyEntries))
            .ToArray();
        if (words.Length == 0)
            return new NoWords();
        return new Verb(words);
    }

    public static implicit operator Verb(string s) => new Verb([s]);

    public static implicit operator Verb(string[] s) => new Verb(s);

    public Verb AddPrefix(string[] prefix)
        => new Verb(prefix.Concat(_verb).ToArray());

    public Verb AddPrefix(string prefix)
        => new Verb(new[] { prefix }.Concat(_verb).ToArray());

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

    public abstract record Error(string Message);
    public sealed record NoWords() : Error(_errorNoWords);
    public sealed record InvalidHandler() : Error(_errorInvalidHandler);
    public sealed record IncorrectFormat() : Error("Input string is not in a parseable format");
}
