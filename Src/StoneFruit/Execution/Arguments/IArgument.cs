using System;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Represents a single argument, either positional, named, or otherwise
    /// </summary>
    public interface IArgument
    {
        /// <summary>
        /// The raw string value of the argument
        /// </summary>
        string Value { get; }

        /// <summary>
        /// True if this value has already been consumed. A consumed argument cannot be retrieved again
        /// </summary>
        bool Consumed { get; }

        /// <summary>
        /// Mark the value as being consumed
        /// </summary>
        /// <returns></returns>
        IArgument MarkConsumed(bool consumed = true);

        /// <summary>
        /// Get the value of the argument as a string, with a default value if the argument doesn't exist.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string AsString(string defaultValue = null);

        /// <summary>
        /// Get the value of the argument as a boolean, with a default value if the argument doesn't exist
        /// or can't be converted.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool AsBool(bool defaultValue = false);

        /// <summary>
        /// Get the value of the argument as an integer, with a default value if the argument doesn't exist
        /// or can't be converted.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        int AsInt(int defaultValue = 0);

        /// <summary>
        /// Get the value of the argument as a long, with a default value if the argument doesn't exist or
        /// can't be converted.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        long AsLong(long defaultValue = 0L);
    }

    public static class ArgumentExtensions
    {
        /// <summary>
        /// Throw an exception if the argument does not exist.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static IArgument Require(this IArgument argument)
        {
            Assert.ArgumentNotNull(argument, nameof(argument));
            (argument as MissingArgument)?.Throw();
            return argument;
        }

        /// <summary>
        /// Returns true if the argument exists, false otherwise
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool Exists(this IArgument argument)
            => argument != null && !(argument is MissingArgument);

        /// <summary>
        /// Helper method to convert the value of the argument to a different format, using a default
        /// value if the conversion fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <param name="transform"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T As<T>(this IArgument argument, Func<string, T> transform, T defaultValue)
        {
            if (argument is MissingArgument)
                return defaultValue;
            try
            {
                return transform(argument.Value);
            }
            catch
            {
                return default;
            }
        }
    }
}