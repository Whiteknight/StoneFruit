using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Provides access to a list of IArgument objects but name or position.
    /// </summary>
    public class CommandArguments
    {
        private readonly IReadOnlyList<PositionalArgument> _positionals;
        private readonly IReadOnlyCollection<string> _flags;
        private readonly IReadOnlyDictionary<string, List<NamedArgument>> _nameds;

        private int _positionalIndex;

        public CommandArguments()
        {
            _positionalIndex = 0;
            _positionals = new PositionalArgument[0];
            _nameds = new Dictionary<string, List<NamedArgument>>();
            _flags = new List<string>();
        }

        public CommandArguments(IEnumerable<IArgument> arguments)
        {
            // TODO: Handle FlagArguments
            var positionals = new List<PositionalArgument>();
            var nameds = new Dictionary<string, List<NamedArgument>>();
            var flags = new HashSet<string>();
            foreach (var arg in arguments)
            {
                if (arg is PositionalArgument positional)
                {
                    positionals.Add(positional);
                    continue;
                }

                if (arg is NamedArgument named)
                {
                    var name = named.Name;
                    if (!nameds.ContainsKey(name))
                        nameds.Add(name, new List<NamedArgument>());
                    nameds[name].Add(named);
                    continue;
                }

                if (arg is FlagArgument flag)
                {
                    flags.Add(flag.Name);
                    continue;
                }
            }

            // TODO: Also keep track of the relative ordering of all args, so that we could have 
            // sequences like "-x value" for certain parsers

            _positionals = positionals;
            _nameds = nameds;
            _flags = flags;
            _positionalIndex = 0;
        }

        public static CommandArguments Single(string arg)
        {
            return new CommandArguments(new[] { new PositionalArgument(arg) });
        }

        public IArgument Shift()
        {
            while (_positionalIndex < _positionals.Count)
            {
                var arg = _positionals[_positionalIndex];
                _positionalIndex++;
                if (!arg.Consumed)
                    return arg;
            }

            return new MissingArgument("Cannot get next positional argument, there are none left. Either you did not pass enough or you consumed them all already.");
        }

        public IArgument Get(int index)
        {
            if (index >= _positionals.Count)
                return new MissingArgument($"Cannot get argument at position {index}. Not enough arguments were provided");
            return _positionals[index];
        }

        public IArgument Get(string name)
        {
            if (!_nameds.ContainsKey(name))
                return new MissingArgument($"Cannot get argument named '{name}'");
            return _nameds[name].FirstOrDefault(a => !a.Consumed);
        }

        public IEnumerable<IArgument> GetAll(string name)
        {
            if (_nameds.ContainsKey(name))
                return Enumerable.Empty<IArgument>();
            return _nameds[name].Where(a => !a.Consumed);
        }

        public IEnumerable<IArgument> GetAllPositionals()
        {
            return _positionals.Where(a => !a.Consumed);
        }

        public bool HasFlag(string name)
        {
            return _flags.Contains(name);
        }

        // TODO: Method to map argument values to an argument object. Mapper should be pluggable
    }
}