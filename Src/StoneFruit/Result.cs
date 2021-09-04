using System;

namespace StoneFruit
{
    /// <summary>
    /// A result of an operation. On success contains a value. On failure there is no value and
    /// attempt to access the value will throw an exception.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResult<T>
    {
        bool HasValue { get; }
        T Value { get; }

        bool Equals(T value);

        T GetValueOrDefault(T defaultValue);

        IResult<TOut> Transform<TOut>(Func<T, TOut> transform);
    }

    /// <summary>
    /// Static class for result factory methods.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Create a success result from the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IResult<T> Success<T>(T value)
            => new SuccessResult<T>(value);

        /// <summary>
        /// Create a failure result of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IResult<T> Fail<T>()
            => FailureResult<T>.Instance;
    }

    /// <summary>
    /// Represents a successful result with a valid value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// Represents a failure result without a valid value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
