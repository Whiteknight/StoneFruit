using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    public class CommandArguments
    {
        private readonly IReadOnlyList<PositionalArgument> _positionals;
        private readonly IReadOnlyDictionary<string, List<NamedArgument>> _nameds;

        private int _positionalIndex;

        public CommandArguments()
        {
            _positionalIndex = 0;
            _positionals = new PositionalArgument[0];
            _nameds = new Dictionary<string, List<NamedArgument>>();
        }

        public CommandArguments(IEnumerable<IArgument> arguments)
        {
            var positionals = new List<PositionalArgument>();
            var nameds = new Dictionary<string, List<NamedArgument>>();
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
                }
            }

            _positionals = positionals;
            _nameds = nameds;
            _positionalIndex = 0;
        }

        public IArgument ShiftNextPositional()
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
    }
}