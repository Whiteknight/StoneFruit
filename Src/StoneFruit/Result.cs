using System;

namespace StoneFruit
{
    public interface IResult<T>
    {
        bool HasValue { get; }
        T Value { get; }

        bool Equals(T value);

        T GetValueOrDefault(T defaultValue);

        IResult<TOut> Transform<TOut>(Func<T, TOut> transform);
    }

    public static class Result
    {
        public static IResult<T> Success<T>(T value)
            => new SuccessResult<T>(value);
    }

    public sealed class SuccessResult<T> : IResult<T>
    {
        public SuccessResult(T value)
        {
            Value = value;
        }

        public bool HasValue => true;
        public T Value { get; }

        public bool Equals(T value)
            => Value!.Equals(value);

        public T GetValueOrDefault(T defaultValue) => Value;

        public IResult<TOut> Transform<TOut>(Func<T, TOut> transform)
        {
            var newValue = transform(Value);
            return new SuccessResult<TOut>(newValue);
        }
    }

    public sealed class FailureResult<T> : IResult<T>
    {
        public static IResult<T> Instance { get; } = new FailureResult<T>();

        public bool HasValue => false;
        public T Value => throw new InvalidOperationException("Cannot access value of failure result");

        public bool Equals(T value) => false;

        public T GetValueOrDefault(T defaultValue) => defaultValue;

        public IResult<TOut> Transform<TOut>(Func<T, TOut> transform) => FailureResult<TOut>.Instance;
    }
}
