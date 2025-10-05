using System;

namespace StoneFruit;

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

    public static implicit operator Result<T, TError>(T value)
        => new Result<T, TError>(value, default, true);

    public static implicit operator Result<T, TError>(TError error)
        => new Result<T, TError>(default, error, false);

    public static Result<T, TError> Create(T value)
        => new Result<T, TError>(value, default, true);

    public static Result<T, TError> Create(TError error)
        => new Result<T, TError>(default, error, false);

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

    public Result<TOut, TError> Map<TOut, TData>(TData data, Func<T, TData, TOut> onValue)
        => Match(
            v => new Result<TOut, TError>(onValue(v, data), default, true),
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

    public Result<T, TError> OnSuccess(Action<T> onSuccess)
    {
        if (_hasValue && _value is not null)
            onSuccess(_value);
        return this;
    }

    public Result<T, TError> OnFailure(Action<TError> onFailure)
    {
        if ((!_hasValue || _value is null) && _error is not null)
            onFailure(_error);
        return this;
    }

    public bool Satisfies(Func<T, bool> predicate)
        => Match(t => predicate(t), _ => false);

    public bool SatisfiesError(Func<TError, bool> predicate)
        => Match(_ => false, e => predicate(e));

    public Result<T, TError> Or(Func<Result<T, TError>> onFailure)
        => Match(v => new Result<T, TError>(v, default, true), _ => onFailure());

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

    public static Result<T, TError> Combine<T, TError>(this Result<T, TError> r1, Result<T, TError> r2, Func<T, T, T> combineValues, Func<TError, TError, TError> combineErrors)
    {
        if (!r1.IsSuccess && !r2.IsSuccess)
            return combineErrors(r1.GetErrorOrThrow(), r2.GetErrorOrThrow());
        if (!r1.IsSuccess)
            return r1;
        if (!r2.IsSuccess)
            return r2;
        return combineValues(r1.GetValueOrThrow(), r2.GetValueOrThrow());
    }
}
