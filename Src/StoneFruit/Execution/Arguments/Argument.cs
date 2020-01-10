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

        public static bool Exists(this IArgument argument) => argument != null && !(argument is MissingArgument);

        public static bool AsBool(this IArgument argument) => As(argument, bool.Parse);

        public static int AsInt(this IArgument argument) => As(argument, int.Parse);

        public static long AsLong(this IArgument argument) => As(argument, long.Parse);

        public static T As<T>(this IArgument argument, Func<string, T> transform)
        {
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