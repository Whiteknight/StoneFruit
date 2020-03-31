using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Provides access to a list of IArgument objects but name or position.
    /// </summary>
    public partial class CommandArguments
    {
        private readonly List<IParsedArgument> _rawArguments;
        private int _accessedShiftIndex;

        // TODO: Would like to do some book-keeping to avoid multiple traversals of the _raw list
        // TODO: Would like to clean the _raw list of nulls regularly

        // Empty args object with no values in it
        public CommandArguments()
        {
            Raw = string.Empty;
            _accessedPositionals = new List<IPositionalArgument>();
            _accessedNameds = new Dictionary<string, List<INamedArgument>>();
            _accessedFlags = new Dictionary<string, IFlagArgument>();
            _rawArguments = new List<IParsedArgument>();
            _accessedShiftIndex = -1;
        }

        // Args object built from existing IArguments. Used when we're already past the point
        // of parsing and accessing
        public CommandArguments(IReadOnlyList<IArgument> arguments)
        {
            Raw = string.Empty;
            
            _accessedPositionals = arguments
                .OfType<IPositionalArgument>()
                .ToList();

            _accessedNameds = arguments.OfType<INamedArgument>()
                .GroupBy(a => a.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            _accessedFlags = arguments
                .OfType<IFlagArgument>()
                .ToDictionary(a => a.Name);

            _rawArguments = new List<IParsedArgument>();
            _accessedShiftIndex = 0;
        }

        // Args object built from parsed objects, which aren't accessed yet
        public CommandArguments(IEnumerable<IParsedArgument> arguments)
            : this(string.Empty, arguments)
        {
        }

        // Args object built from parsed objects, which aren't accessed yet
        public CommandArguments(string rawArgs, IEnumerable<IParsedArgument> arguments)
        {
            Raw = rawArgs;
            _accessedPositionals = new List<IPositionalArgument>();
            _accessedNameds = new Dictionary<string, List<INamedArgument>>();
            _accessedFlags = new Dictionary<string, IFlagArgument>();
            _rawArguments = arguments.ToList();
            _accessedShiftIndex = -1;
        }

        /// <summary>
        /// The raw, unparsed argument string if available
        /// </summary>
        public string Raw { get; }

        /// <summary>
        /// A default, empty arguments object
        /// </summary>
        /// <returns></returns>
        public static CommandArguments Empty() => new CommandArguments();

        /// <summary>
        /// An arguments object for a single value
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static CommandArguments Single(string arg) 
            => new CommandArguments(arg, new IParsedArgument[] { new PositionalArgument(arg) });

        //public void VerifyAllAreConsumed()
        //{
        //    if (_rawArguments.Any())

        //    var unconsumed = GetAllArguments().ToList();
        //    if (!unconsumed.Any())
        //        return;
        //    var sb = new StringBuilder();
        //    sb.AppendLine("Arguments were provided which were not consumed.");
        //    sb.AppendLine();
        //    foreach (var u in unconsumed)
        //    {
        //        var str = u switch
        //        {
        //            PositionalArgument p => p.AsString(),
        //            NamedArgument n => $"'{n.Name}' = {n.AsString()}",
        //            FlagArgument f => $"flag {f.Name}",
        //            _ => "Unknown"
        //        };
        //        sb.AppendLine(str);
        //    }

        //    throw new CommandArgumentException(sb.ToString());
        //}

        // TODO: Method to create a sub-CommandArguments instance with some arguments added/removed

        /// <summary>
        /// Create a new instance of type T and attempt to map argument values into the
        /// public properties of the new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T MapTo<T>()
            where T : new() 
            => new CommandArgumentMapper().Map<T>(this);

        /// <summary>
        /// Attempt to map argument values onto the public properties of the given object
        /// instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void MapOnto<T>(T obj)
            => new CommandArgumentMapper().MapOnto<T>(this, obj);

        /// <summary>
        /// Get a list of all argument objects
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IArgument> GetAllArguments()
            => GetAllPositionals().Cast<IArgument>()
                .Concat(GetAllNamed())
                .Concat(GetAllFlags())
                .Where(p => !p.Consumed);

        /// <summary>
        /// Resets the Consumed state of all arguments
        /// </summary>
        public void ResetAllArguments()
        {
            foreach (var p in _accessedPositionals)
                p.MarkConsumed(false);
            foreach (var n in _accessedNameds.Values.SelectMany(x => x))
                n.MarkConsumed(false);
            foreach (var f in _accessedFlags.Values)
                f.MarkConsumed(false);
        }
    }
}