using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StoneFruit.Execution;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// Information about a registered verb.
/// </summary>
public interface IVerbInfo
{
    /// <summary>
    /// Gets the verb used to invoke the handler. All verbs are case-insensitive.
    /// </summary>
    Verb Verb { get; }

    /// <summary>
    /// Gets a short description of the verb.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets detailed usage information about the verb.
    /// </summary>
    string Usage { get; }

    /// <summary>
    /// Gets the grouping that the verb belongs in.
    /// </summary>
    string Group { get; }

    /// <summary>
    /// Gets a value indicating whether whether or not to show the verb in the list when executing "help".
    /// </summary>
    bool ShouldShowInHelp { get; }
}

/// <summary>
/// Get a list of possible verbs from an IHandlerBase Type.
/// </summary>
public interface IVerbExtractor
{
    /// <summary>
    /// Gets a list of possible verbs from the type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Should not return null.</returns>
    Result<Verb[], Verb.Error> GetVerbs(Type type);

    /// <summary>
    /// Gets a list of possible verbs from the method.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    Result<Verb[], Verb.Error> GetVerbs(MethodInfo method);
}

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
        => Validate.IsNotNullOrEmpty(verb)
            .ToResult(() => NoWords)
            .Map(SplitOnSpaces)
            .Bind(TryCreate);

    public static Result<Verb, Error> TryCreate(string[] verb)
    {
        var words = verb
            .OrEmptyIfNull()
            .SelectMany(SplitOnSpaces)
            .Where(w => !string.IsNullOrEmpty(w))
            .ToArray();
        return words.Length == 0
            ? NoWords
            : new Verb(words);
    }

    public static implicit operator Verb(string s) => new Verb([s]);

    public static implicit operator Verb(string[] s) => new Verb(s);

    public bool IsValid => _verb?.Length > 0;

    public Verb AddPrefix(string[] prefix)
        => prefix == null || prefix.Length == 0 || prefix.All(string.IsNullOrWhiteSpace)
            ? this
            : new Verb(prefix.Concat(_verb).ToArray());

    public Verb AddPrefix(string? prefix)
        => string.IsNullOrEmpty(prefix)
            ? this
            : new Verb(SplitOnSpaces(prefix).Concat(_verb).ToArray());

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

    private static string[] SplitOnSpaces(string v)
    {
        if (v == null)
            return [];
        return v.Contains(' ')
            ? v.Split(Constants.SeparatedBySpace, StringSplitOptions.RemoveEmptyEntries)
            : [v];
    }

    public static Error NoWords { get; } = new NoWordsError();

    public static Error InvalidHandler { get; } = new InvalidHandlerError();

    public static Error IncorrectFormat { get; } = new IncorrectFormatError();

    public abstract record Error(string Message);

    public sealed record NoWordsError() : Error(_errorNoWords);

    public sealed record InvalidHandlerError() : Error(_errorInvalidHandler);

    public sealed record IncorrectFormatError() : Error("Input string is not in a parseable format");
}
