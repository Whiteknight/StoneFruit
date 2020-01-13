using System;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Represents a single argument, either positional, named, or otherwise
    /// </summary>
    public interface IArgument
    {
        string Value { get; }
        bool Consumed { get; }
        IArgument MarkConsumed();
    }

    public static class ArgumentExtensions
    {
        public static IArgument Require(this IArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));
            (argument as MissingArgument)?.Throw();
            return argument;
        }

        public static string ValueOrDefault(this IArgument argument, string defaultValue = null) => string.IsNullOrEmpty(argument.Value) ? defaultValue : argument.Value;

        public static bool Exists(this IArgument argument) => argument != null && !(argument is MissingArgument);

        public static bool AsBool(this IArgument argument, bool defaultValue = false) => As(argument, bool.Parse, defaultValue);

        public static int AsInt(this IArgument argument, int defaultValue = 0) => As(argument, int.Parse, defaultValue);

        public static long AsLong(this IArgument argument, long defaultValue = 0L) => As(argument, long.Parse, defaultValue);

        public static T As<T>(this IArgument argument, Func<string, T> transform, T defaultValue)
        {
            try
            {
                if (string.IsNullOrEmpty(argument?.Value))
                    return defaultValue;
                return transform(argument.Value);
            }
            catch
            {
                return default;
            }
        }
    }
}