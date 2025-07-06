using System;
using System.Collections.Generic;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public readonly struct Maybe<T>
{
    private readonly T? _value;
    private readonly bool _hasValue;

    public Maybe(T? value)
    {
        _value = value;
        _hasValue = value is not null;
    }

    public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

    public bool IsSuccess => _hasValue && _value is not null;

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TOut> onFailure)
        => _hasValue && _value is not null
            ? onSuccess(_value)
            : onFailure();

    public Maybe<TOut> Map<TOut>(Func<T, TOut> onSuccess)
        => Match<Maybe<TOut>>(v => onSuccess(v), static () => default);

    public T GetValueOrDefault(T defaultValue = default!)
    {
        var result = Match(v => v, () => defaultValue);
        return result is not null
            ? result
            : throw new InvalidOperationException("Attempt to return null from .GetValueOrDefault()");
    }

    public T GetValueOrThrow()
        => Match(v => v, () => throw new InvalidOperationException("Could not get value of Maybe"));

    public Result<T, TError> ToResult<TError>(Func<TError> createError)
        => Match(v => (Result<T, TError>)v, () => (Result<T, TError>)createError());

    public Maybe<T> OnSuccess(Action<T> onSuccess)
    {
        if (_hasValue && _value is not null)
            onSuccess(_value);
        return this;
    }

    public Maybe<T> OnFailure(Action onFailure)
    {
        if (!_hasValue || _value is null)
            onFailure();
        return this;
    }
}

public static class Maybe
{
    public static Maybe<TOut> Bind<T, TOut>(this Maybe<T> maybe, Func<T, Maybe<TOut>> onSuccess)
        => maybe.Match(v => onSuccess(v), static () => default);

    public static Maybe<T> Flatten<T>(this Maybe<Maybe<T>> maybe)
        => maybe.Bind(static v => v);

    public static bool Is<T>(this Maybe<T> maybe, T expected)
        => maybe.Match(v => v!.Equals(expected), () => false);

    public static bool Is<T>(this Maybe<T> maybe, Func<T, bool> predicate)
        => maybe.Match(predicate, () => false);

    public static Maybe<T> Or<T>(this Maybe<T> maybe, Func<Maybe<T>> onFailure)
        => maybe.Match(v => new Maybe<T>(v), () => onFailure());

    public static Maybe<T> Where<T>(this Maybe<T> maybe, Func<T, bool> predicate)
        => maybe.Bind(v => predicate(v) ? new Maybe<T>(v) : default);
}

public static class MaybeExtensions
{
    public static Maybe<TValue> MaybeGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull
        => NotNull(dict).TryGetValue(key, out var value) && value is not null ? value : default(Maybe<TValue>);
}

public readonly struct Result<T, TError>
{
    private readonly T? _value;
    private readonly TError? _error;
    private readonly bool _hasValue;

    public Result(T? value, TError? error, bool hasValue)
    {
        _value = value;
        _error = error;
        _hasValue = hasValue && _value is not null;
    }

    public static implicit operator Result<T, TError>(T value) => new Result<T, TError>(value, default, true);

    public static implicit operator Result<T, TError>(TError error) => new Result<T, TError>(default, error, false);

    public bool IsSuccess => _hasValue && _value is not null;

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TError, TOut> onError)
    {
        if (_hasValue && _value is not null)
            return onSuccess(_value);
        if (_error is not null)
            return onError(_error);
        throw new InvalidOperationException("Result does not have a valid Value or Error");
    }

    public Result<TOut, TError> Map<TOut>(Func<T, TOut> onValue)
        => Match(
            v => new Result<TOut, TError>(onValue(v), default, true),
            e => new Result<TOut, TError>(default, e, false));

    public Result<T, TErrorOut> MapError<TErrorOut>(Func<TError, TErrorOut> onError)
        => Match(
            v => new Result<T, TErrorOut>(v, default, true),
            e => new Result<T, TErrorOut>(default, onError(e), false));

    public T? GetValueOrDefault(T? defaultValue = default)
        => Match(v => v, _ => defaultValue);

    public T GetValueOrThrow()
        => Match(
            v => v,
            e => throw new InvalidOperationException($"Could not get value of Result. Error: {e}"));

    public TError? GetErrorOrDefault(TError? defaultValue = default)
        => Match(_ => defaultValue, e => e);

    public TError GetErrorOrThrow()
        => Match(
            _ => throw new InvalidOperationException("Could not get Error of Result."),
            e => e);

    public void ThrowIfError()
    {
        if (_hasValue)
            return;
        if (_error is System.Exception ex)
            throw new Result<T, TError>.Exception(ex);
        if (_error is null)
            throw new InvalidOperationException("Could not get the Error of Result.");
        throw new Exception(_error);
    }

    public class Exception : System.Exception
    {
        public Exception(object error)
            : base(error.ToString())
        {
        }

        public Exception(System.Exception inner)
            : base(inner.Message, inner)
        {
        }
    }
}

public static class Result
{
    public static Result<TOut, TError> Bind<T, TError, TOut>(this Result<T, TError> result, Func<T, Result<TOut, TError>> onSuccess)
        => result.Match(v => onSuccess(v), e => new Result<TOut, TError>(default, e, false));
}