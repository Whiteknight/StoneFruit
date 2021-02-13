using System;

namespace StoneFruit
{
    public interface IResult<out T>
    {
        bool HasValue { get; }
        T Value { get; }

        IResult<TOut> Transform<TOut>(Func<T, TOut> transform);
    }

    public static class Result
    {
        public static IResult<T> Success<T>(T value)
            => new SuccessResult<T>(value);
    }

    public class SuccessResult<T> : IResult<T>
    {
        public SuccessResult(T value)
        {
            Value = value;
        }

        public bool HasValue => true;
        public T Value { get; }

        public IResult<TOut> Transform<TOut>(Func<T, TOut> transform)
        {
            var newValue = transform(Value);
            return new SuccessResult<TOut>(newValue);
        }
    }

    public class FailureResult<T> : IResult<T>
    {
        public static IResult<T> Instance { get; } = new FailureResult<T>();

        public bool HasValue => false;
        public T Value => throw new InvalidOperationException("Cannot access value of failure result");

        public IResult<TOut> Transform<TOut>(Func<T, TOut> transform) => FailureResult<TOut>.Instance;
    }
}
