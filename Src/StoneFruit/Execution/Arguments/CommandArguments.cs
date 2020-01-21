﻿using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Provides access to a list of IArgument objects but name or position.
    /// </summary>
    public class CommandArguments
    {
        private readonly IReadOnlyList<PositionalArgument> _positionals;
        private readonly IReadOnlyDictionary<string, FlagArgument> _flags;
        private readonly IReadOnlyDictionary<string, List<NamedArgument>> _nameds;

        private int _positionalIndex;

        public CommandArguments()
        {
            _positionalIndex = 0;
            _positionals = new PositionalArgument[0];
            _nameds = new Dictionary<string, List<NamedArgument>>();
            _flags = new Dictionary<string, FlagArgument>();
        }

        public CommandArguments(IReadOnlyList<IArgument> arguments)
        {
            _positionals = arguments
                .OfType<PositionalArgument>()
                .ToList();
            _nameds = arguments
                .OfType<NamedArgument>()
                .GroupBy(n => n.Name.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.ToList());
            _flags = arguments.OfType<FlagArgument>().GroupBy(f => f.Value.ToLowerInvariant()).ToDictionary(g => g.Key, g => g.FirstOrDefault());
            _positionalIndex = 0;
        }

        public static CommandArguments Empty() => new CommandArguments();

        public static CommandArguments Single(string arg) 
            => new CommandArguments(new[] { new PositionalArgument(arg) });

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
            name = name.ToLowerInvariant();
            if (!_nameds.ContainsKey(name))
                return new MissingArgument($"Cannot get argument named '{name}'");
            return _nameds[name].FirstOrDefault(a => !a.Consumed);
        }

        public IEnumerable<IArgument> GetAll(string name)
        {
            name = name.ToLowerInvariant();
            if (_nameds.ContainsKey(name))
                return Enumerable.Empty<IArgument>();
            return _nameds[name].Where(a => !a.Consumed);
        }

        // TODO: GetLike(string part)
        // TODO: GetAllPositionalValues (naming?)
        // TODO: GetAllFlags (naming?)
        // TODO: GetAllNamedValues (naming?)

        public IEnumerable<IArgument> GetAllPositionals() => _positionals.Where(a => !a.Consumed);

        public bool HasFlag(string name)
        {
            name = name.ToLowerInvariant();
            return _flags.ContainsKey(name) && _flags[name] != null && !_flags[name].Consumed;
        }

        // TODO: Method to create a sub-CommandArguments instance with some arguments added/removed

        public T MapTo<T>()
            where T : new() 
            => new CommandArgumentMapper().Map<T>(this);
    }
}