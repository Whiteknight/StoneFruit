using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Provides access to a list of IArgument objects but name or position.
    /// </summary>
    public class CommandArguments
    {
        private readonly List<IPositionalArgument> _accessedPositionals;
        private readonly Dictionary<string, IFlagArgument> _accessedFlags;
        private readonly Dictionary<string, List<INamedArgument>> _accessedNameds;
        private readonly List<IParsedArgument> _rawArguments;

        // TODO: Would like to do some book-keeping to avoid multiple traversals of the _raw list
        // TODO: Would like to clean the _raw list of nulls regularly
        // TODO: Cleanup and see if we can reduce code duplication

        // Empty args object with no values in it
        public CommandArguments()
        {
            Raw = string.Empty;
            _accessedPositionals = new List<IPositionalArgument>();
            _accessedNameds = new Dictionary<string, List<INamedArgument>>();
            _accessedFlags = new Dictionary<string, IFlagArgument>();
            _rawArguments = new List<IParsedArgument>();
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

        /// <summary>
        /// Get the next unaccess positional value
        /// </summary>
        /// <returns></returns>
        public IPositionalArgument Shift() => TryShift() ?? MissingArgument.NoPositionals();

        public IPositionalArgument Get(int index)
        {
            // Check if we have accessed this index already. If so we either return it or
            // it's consumed already
            if (index < _accessedPositionals.Count)
            {
                if (_accessedPositionals[index].Consumed)
                    return MissingArgument.PositionalConsumed(index);
                return _accessedPositionals[index];
            }

            // Loop through the unaccessed args until we either find the one we're looking
            // for or we run out.
            var current = _accessedPositionals.Count - 1;
            while (current < index)
            {
                var ok = TryShift();
                if (ok == null)
                    return MissingArgument.NoPositionals();
                current++;
            }

            return _accessedPositionals[index];
        }

        public IPositionalArgument Consume(int index) => Get(index).MarkConsumed() as IPositionalArgument;

        public INamedArgument Get(string name)
        {
            name = name.ToLowerInvariant();

            // Check the already-accessed named args. If we have it, return it.
            if (_accessedNameds.ContainsKey(name))
            {
                var firstAvailable = _accessedNameds[name].FirstOrDefault(a => !a.Consumed);
                if (firstAvailable != null)
                    return _accessedNameds[name].First();
            }

            // Loop through all unaccessed args looking for the first one with the given
            // name. 
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is NamedArgument n)
                {
                    if (n.Name != name)
                        continue;
                    var accessor = new NamedArgumentAccessor(n.Name, n.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                    return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument n2)
                {
                    if (n2.Name != name)
                        continue;
                    var accessor = new NamedArgumentAccessor(n2.Name, n2.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                    return accessor;
                }
            }

            // Not found
            return MissingArgument.NoneNamed(name);
        }

        public INamedArgument Consume(string name) => Get(name).MarkConsumed() as INamedArgument;

        public IFlagArgument GetFlag(string name)
        {
            name = name.ToLowerInvariant();

            // Check if we've already accessed this flag. If so, return it if unconsumed
            // or not found
            if (_accessedFlags.ContainsKey(name))
            {
                if (_accessedFlags[name].Consumed)
                    return MissingArgument.FlagConsumed(name);
                return _accessedFlags[name];
            }

            // Loop through unaccessed args looking for a matching flag.
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is FlagArgument f)
                {
                    if (f.Name != name)
                        continue;
                    var accessor = new FlagArgumentAccessor(f.Name);
                    _rawArguments[i] = null;
                    _accessedFlags.Add(name, accessor);
                    return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    if (fp.Name != name)
                        continue;
                    var accessor = new FlagArgumentAccessor(fp.Name);
                    _accessedFlags.Add(name, accessor);
                    _rawArguments[i] = new PositionalArgument(fp.Value);
                    return accessor;
                }
            }

            // Not found
            return MissingArgument.FlagMissing(name);
        }

        public IFlagArgument ConsumeFlag(string name) => GetFlag(name).MarkConsumed() as IFlagArgument;

        public bool HasFlag(string name, bool markConsumed = false)
        {
            var flag = GetFlag(name);
            if (!flag.Exists())
                return false;
            if (markConsumed)
                flag.MarkConsumed();
            return true;
        }

        public IEnumerable<IArgument> GetAll(string name)
        {
            name = name.ToLowerInvariant();

            // Loop through all unaccessed args, making sure all args with this name
            // are in the accessed loop
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is NamedArgument n)
                {
                    if (n.Name != name)
                        continue;
                    var accessor = new NamedArgumentAccessor(n.Name, n.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                }

                if (arg is FlagPositionalOrNamedArgument n2)
                {
                    if (n2.Name != name)
                        continue;
                    var accessor = new NamedArgumentAccessor(n2.Name, n2.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                }
            }

            // return accessed items with this name, which may be the empty set
            if (_accessedNameds.ContainsKey(name))
                return _accessedNameds[name].Where(a => !a.Consumed);
            return Enumerable.Empty<IArgument>();
        }

        public IEnumerable<IPositionalArgument> GetAllPositionals()
        {
            // Loop over the unaccessed args, accessing all positionals
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is PositionalArgument pa)
                {
                    var accessor = new PositionalArgumentAccessor(pa.Value);
                    _rawArguments[i] = null;
                    _accessedPositionals.Add(accessor);
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    var accessor = new PositionalArgumentAccessor(fp.Value);
                    _accessedPositionals.Add(accessor);
                    var flag = new FlagArgument(fp.Name);
                    _rawArguments[i] = flag;
                }
            }

            // Return
            return _accessedPositionals.Where(a => !a.Consumed);
        }

        public IEnumerable<INamedArgument> GetAllNamed()
        {
            // Access all named arguments
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is NamedArgument n)
                {
                    var accessor = new NamedArgumentAccessor(n.Name, n.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                }

                if (arg is FlagPositionalOrNamedArgument n2)
                {
                    var accessor = new NamedArgumentAccessor(n2.Name, n2.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                }
            }
            return _accessedNameds.Values.SelectMany(n => n).Where(a => !a.Consumed);
        }

        public IEnumerable<IFlagArgument> GetAllFlags()
        {
            // Access all flags
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is FlagArgument f)
                {
                    var accessor = new FlagArgumentAccessor(f.Name);
                    _rawArguments[i] = null;
                    _accessedFlags.Add(f.Name, accessor);
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    var accessor = new FlagArgumentAccessor(fp.Name);
                    _accessedFlags.Add(fp.Name, accessor);
                    _rawArguments[i] = new PositionalArgument(fp.Value);
                }
            }
            return _accessedFlags.Values.Where(a => !a.Consumed);
        }

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

        public T MapTo<T>()
            where T : new() 
            => new CommandArgumentMapper().Map<T>(this);

        public IEnumerable<IArgument> GetAllArguments()
            => GetAllPositionals().Cast<IArgument>()
                .Concat(GetAllNamed())
                .Concat(GetAllFlags())
                .Where(p => !p.Consumed);

        public void ResetAllArguments()
        {
            foreach (var p in _accessedPositionals)
                p.MarkConsumed(false);
            foreach (var n in _accessedNameds.Values.SelectMany(x => x))
                n.MarkConsumed(false);
            foreach (var f in _accessedFlags.Values)
                f.MarkConsumed(false);
        }

        private IPositionalArgument TryShift()
        {
            // TODO: If we constructed with IArgument instead of IParsedArgument this won't work
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is PositionalArgument pa)
                {
                    var accessor = new PositionalArgumentAccessor(pa.Value);
                    _rawArguments[i] = null;
                    _accessedPositionals.Add(accessor);
                    return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    var accessor = new PositionalArgumentAccessor(fp.Value);
                    _accessedPositionals.Add(accessor);
                    var flag = new FlagArgument(fp.Name);
                    _rawArguments[i] = flag;
                    return accessor;
                }
            }

            return null;
        }

        private void AccessNamed(NamedArgumentAccessor n)
        {
            if (!_accessedNameds.ContainsKey(n.Name))
                _accessedNameds.Add(n.Name, new List<INamedArgument>());
            _accessedNameds[n.Name].Add(n);
        }
    }
}