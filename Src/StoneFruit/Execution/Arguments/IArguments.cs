using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    public interface IArguments
    {
        /// <summary>
        /// The raw, unparsed argument string if available
        /// </summary>
        string Raw { get; }

        /// <summary>
        /// Resets the Consumed state of all arguments
        /// </summary>
        void ResetAllArguments();

        /// <summary>
        /// Get the next positional value
        /// </summary>
        /// <returns></returns>
        IPositionalArgument Shift();

        IPositionalArgument Get(int index);
        IEnumerable<IPositionalArgument> GetAllPositionals();
        IFlagArgument GetFlag(string name);
        bool HasFlag(string name, bool markConsumed = false);
        IEnumerable<IFlagArgument> GetAllFlags();
        INamedArgument Get(string name);
        IEnumerable<IArgument> GetAll(string name);
        IEnumerable<INamedArgument> GetAllNamed();


        void VerifyAllAreConsumed();
    }

    public static class ArgumentsExtensions
    {
        public static IPositionalArgument Consume(this IArguments args, int index) 
            => args.Get(index).MarkConsumed() as IPositionalArgument;

        public static IFlagArgument ConsumeFlag(this IArguments args, string name) 
            => args.GetFlag(name).MarkConsumed() as IFlagArgument;

        public static INamedArgument Consume(this IArguments args, string name) 
            => args.Get(name).MarkConsumed() as INamedArgument;

        /// <summary>
        /// Create a new instance of type T and attempt to map argument values into the
        /// public properties of the new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T MapTo<T>(this IArguments args)
            where T : new()
            => CommandArgumentMapper.Map<T>(args);

        /// <summary>
        /// Attempt to map argument values onto the public properties of the given object
        /// instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="obj"></param>
        public static void MapOnto<T>(this IArguments args, T obj)
            => CommandArgumentMapper.MapOnto<T>(args, obj);

        /// <summary>
        /// Get a list of all argument objects
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IArgument> GetAllArguments(this IArguments args)
            => args.GetAllPositionals().Cast<IArgument>()
                .Concat(args.GetAllNamed())
                .Concat(args.GetAllFlags())
                .Where(p => !p.Consumed);
    }
}