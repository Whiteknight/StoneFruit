﻿using System;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Represents a single argument, either positional, named, or otherwise
    /// </summary>
    public interface IArgument
    {
        /// <summary>
        /// True if this value has already been consumed. A consumed argument cannot be retrieved again
        /// </summary>
        bool Consumed { get; set; }
    }

    /// <summary>
    /// Represents an argument with a value
    /// </summary>
    public interface IValuedArgument : IArgument
    {
        /// <summary>
        /// The raw string value of the argument
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Get the value of the argument as a string, with a default value if the argument doesn't exist.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string AsString(string defaultValue = "");

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

    /// <summary>
    /// A positional argument is a value argument with no name
    /// </summary>
    public interface IPositionalArgument : IValuedArgument
    {
    }

    /// <summary>
    /// A named argument is a value argument with a name
    /// </summary>
    public interface INamedArgument : IValuedArgument
    {
        /// <summary>
        /// The name of the argument
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// A flag argument is an argument with a name but no value
    /// </summary>
    public interface IFlagArgument : IArgument
    {
        /// <summary>
        /// The name flag
        /// </summary>
        string Name { get; }
    }

    public static class ArgumentExtensions
    {
        /// <summary>
        /// Throw an exception if the argument does not exist.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static T Require<T>(this T argument, string errorMessage = "")
            where T : IArgument
        {
            Assert.ArgumentNotNull(argument, nameof(argument));
            (argument as MissingArgument)?.Throw(errorMessage);
            return argument;
        }

        /// <summary>
        /// Mark the value as being consumed
        /// </summary>
        /// <returns></returns>
        public static T MarkConsumed<T>(this T argument, bool consumed = true)
            where T : IArgument
        {
            Assert.ArgumentNotNull(argument, nameof(argument));
            argument.Consumed = consumed;
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
        public static T As<T>(this IValuedArgument argument, Func<string, T> transform, T defaultValue)
        {
            if (argument is MissingArgument)
                return defaultValue;
            try
            {
                return transform(argument.Value);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
