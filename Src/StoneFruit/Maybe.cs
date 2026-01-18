using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public static implicit operator Maybe<T>(T? value) => new Maybe<T>(value);

    public bool IsSuccess => _hasValue && _value is not null;

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TOut> onFailure)
        => _hasValue && _value is not null
            ? onSuccess(_value)
            : onFailure();

    private TOut Match<TOut, TData>(TData data, Func<T, TData, TOut> onSuccess, Func<TData, TOut> onFailure)
        => _hasValue && _value is not null
            ? onSuccess(_value, data)
            : onFailure(data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Maybe<TOut> Map<TOut>(Func<T, TOut> onSuccess)
        => Match<Maybe<TOut>>(v => onSuccess(v), static () => default);

    public T GetValueOrDefault(T defaultValue = default!)
        => Match(defaultValue, static (v, _) => v, static dv => dv) switch
        {
            T result => result,
            _ => throw new InvalidOperationException("Attempt to return null from .GetValueOrDefault()")
        };

    public T GetValueOrThrow()
        => Match(static v => v, () => throw new InvalidOperationException("Could not get value of Maybe"));

    public Result<T, TError> ToResult<TError>(Func<TError> createError)
        => Match(static v => (Result<T, TError>)v, () => (Result<T, TError>)createError());

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

    public bool Satisfies(Func<T, bool> predicate)
        => Match(predicate, static (v, p) => p(v), static _ => false);
}

public static class Maybe
{
    public static Maybe<TOut> And<T, TOut>(this Maybe<T> maybe, Func<T, Maybe<TOut>> onSuccess)
        => maybe.Match(v => onSuccess(v), static () => default);

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

public static class Validate
{
    public static Maybe<string> IsNotNullOrEmpty(string value)
        => string.IsNullOrEmpty(value)
            ? default
            : value;
}
