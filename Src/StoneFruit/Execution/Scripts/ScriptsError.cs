using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Scripts;

public abstract record ScriptsError
{
    public virtual ScriptsError Combine(ScriptsError other)
    {
        if (other is EmptyLine)
            return this;
        if (other is CombinedError combined2)
            return combined2.Combine(this);
        return new CombinedError(this, other);
    }
}

public sealed record MissingRequiredPositional(int Line, int Index) : ScriptsError
{
    public override string ToString() => $"Line {Line}: Required argument at position {Index} was not provided";
}

public sealed record MissingRequiredNamed(int Line, string Name) : ScriptsError
{
    public override string ToString() => $"Line {Line}: Required argument named '{Name}' was not provided";
}

public sealed record EmptyLine : ScriptsError
{
    public override ScriptsError Combine(ScriptsError other) => other;
}

public sealed record CombinedError : ScriptsError
{
    private readonly List<ScriptsError> _errors;

    public CombinedError(ScriptsError error1, ScriptsError error2)
    {
        _errors = [error1, error2];
    }

    public override ScriptsError Combine(ScriptsError other)
    {
        if (other is CombinedError combined)
        {
            _errors.AddRange(combined._errors);
            return this;
        }

        _errors.Add(other);
        return this;
    }

    public override string ToString() => string.Join("\n", _errors.Select(e => e.ToString()));
}

public sealed class ScriptsException : Exception
{
    public ScriptsException(ScriptsError error)
        : base(error.ToString())
    {
    }
}

public static class ResultExtensions
{
    public static T GetValueOrThrowScriptsException<T>(this Result<T, ScriptsError> result)
        => result.Match(
            success => success,
            error => throw new ScriptsException(error));
}