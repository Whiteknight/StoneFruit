﻿using StoneFruit.Utility;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Collection of parsed arguments. All names are case-invariant.
    /// </summary>
    public interface IArguments
    {
        /// <summary>
        /// The raw, unparsed argument string if available. This string is usually only
        /// populated from the parser and is not usually available otherwise.
        /// </summary>
        string Raw { get; }

        /// <summary>
        /// Resets the Consumed state of all arguments. Useful if the arguments object is
        /// going to be reused.
        /// </summary>
        void ResetAllArguments();

        /// <summary>
        /// Get the next positional value in order without regard to whether the argument
        /// has been consumed.
        /// </summary>
        /// <returns></returns>
        IPositionalArgument Shift();

        /// <summary>
        /// Get the positional value at the given index. The first argument is at position
        /// 0. Returns the argument regardless of whether it has been consumed.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPositionalArgument Get(int index);

        /// <summary>
        /// Get the unconsumed named argument with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        INamedArgument Get(string name);

        /// <summary>
        /// Get all remaining unconsumed positional arguments. Arguments which are
        /// ambiguous will be treated as positionals during iteration.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPositionalArgument> GetAllPositionals();

        /// <summary>
        /// Get the unconsumed flag argument with the given name. If the argument has
        /// already been consumed, it will not be returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IFlagArgument GetFlag(string name);

        /// <summary>
        /// Returns true if the collection contains a flag with the given name, regardless
        /// of consumed state
        /// </summary>
        /// <param name="name"></param>
        /// <param name="markConsumed"></param>
        /// <returns></returns>
        bool HasFlag(string name, bool markConsumed = false);

        /// <summary>
        /// Get all remaining unconsumed flag arguments. Arguments which are ambiguous
        /// will be treated as flags during iteration.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IFlagArgument> GetAllFlags();

        /// <summary>
        /// Get all remaining unconsumed arguments of all types. Arguments which are
        /// ambiguous will be resolved in the order positional, named, flags.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<IArgument> GetAll(string name);

        /// <summary>
        /// Get all remaining unconsumed named arguments.
        /// </summary>
        /// <returns></returns>
        IEnumerable<INamedArgument> GetAllNamed();

        /// <summary>
        /// Throw an exception if any arguments have not been consumed. Useful to alert
        /// the user if extra/unnecessary arguments have been passed.
        /// </summary>
        void VerifyAllAreConsumed();
    }

    public static class ArgumentsExtensions
    {
        /// <summary>
        /// Get the positional argument at the given index, and mark it consumed
        /// </summary>
        /// <param name="args"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IPositionalArgument Consume(this IArguments args, int index)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            return args.Get(index).MarkConsumed();
        }

        /// <summary>
        /// Get the flag argument with the given name and mark it consumed
        /// </summary>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static INamedArgument Consume(this IArguments args, string name)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            return args.Get(name).MarkConsumed();
        }

        /// <summary>
        /// Get the named argument with the given name and mark it consumed
        /// </summary>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFlagArgument ConsumeFlag(this IArguments args, string name)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            return args.GetFlag(name).MarkConsumed();
        }

        /// <summary>
        /// Create a new instance of type T and attempt to map argument values into the
        /// public properties of the new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T MapTo<T>(this IArguments args)
            where T : new()
        {
            Assert.ArgumentNotNull(args, nameof(args));
            return CommandArgumentMapper.Map<T>(args);
        }

        /// <summary>
        /// Attempt to map argument values onto the public properties of the given object
        /// instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="obj"></param>
        public static void MapOnto<T>(this IArguments args, T obj)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            CommandArgumentMapper.MapOnto<T>(args, obj);
        }

        /// <summary>
        /// Get a list of all argument objects
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IArgument> GetAllArguments(this IArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            return args.GetAllPositionals().Cast<IArgument>()
                .Concat(args.GetAllNamed())
                .Concat(args.GetAllFlags())
                .Where(p => !p.Consumed);
        }
    }
}