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
        string AsString(string defaultValue = null);
        bool AsBool(bool defaultValue = false);
        int AsInt(int defaultValue = 0);
        long AsLong(long defaultValue = 0L);
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

        public static bool Exists(this IArgument argument)
            => argument != null && !(argument is MissingArgument);

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