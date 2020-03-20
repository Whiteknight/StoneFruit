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
            Raw = string.Empty;
            _positionals = arguments
                .OfType<PositionalArgument>()
                .ToList();
            _nameds = arguments
                .OfType<NamedArgument>()
                .GroupBy(n => n.Name.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.ToList());
            _flags = arguments
                .OfType<FlagArgument>()
                .GroupBy(f => f.Name.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());
            _positionalIndex = 0;
        }

        public CommandArguments(string rawArgs, IReadOnlyList<IArgument> arguments)
        {
            Raw = rawArgs;
            _positionals = arguments
                .OfType<PositionalArgument>()
                .ToList();
            _nameds = arguments
                .OfType<NamedArgument>()
                .GroupBy(n => n.Name.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.ToList());
            _flags = arguments
                .OfType<FlagArgument>()
                .GroupBy(f => f.Name.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());
            _positionalIndex = 0;
        }

        public string Raw { get; }

        public static CommandArguments Empty() => new CommandArguments();

        public static CommandArguments Single(string arg) 
            => new CommandArguments(arg, new[] { new PositionalArgument(arg) });

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
            // TODO: Option to mark consumed
            // TODO: Should filter out Consumed, like Get(name) and HasFlag(name) do
            if (index >= _positionals.Count)
                return new MissingArgument($"Cannot get argument at position {index}. Not enough arguments were provided");
            return _positionals[index];
        }

        public IArgument Get(string name)
        {
            // TODO: Option to mark consumed
            name = name.ToLowerInvariant();
            if (!_nameds.ContainsKey(name))
                return new MissingArgument($"Cannot get argument named '{name}'");
            return _nameds[name].FirstOrDefault(a => !a.Consumed) ?? (IArgument)new MissingArgument($"Cannot get argument named '{name}'");
        }

        public IArgument GetFlag(string name)
        {
            name = name.ToLowerInvariant();
            if (!_flags.ContainsKey(name) || _flags[name].Consumed)
                return new MissingArgument($"Cannot get flag named '{name}'");
            return _flags[name];
        }

        public IEnumerable<IArgument> GetAll(string name)
        {
            // TODO: Option to mark consumed
            name = name.ToLowerInvariant();
            if (!_nameds.ContainsKey(name))
                return Enumerable.Empty<IArgument>();
            return _nameds[name].Where(a => !a.Consumed);
        }

        // TODO: GetLike(string part)
        // TODO: GetAllPositionalValues (naming?) gets the values not the IARgument
        // TODO: GetAllNamedValues (naming?) gets the values not the IArgument
        // TODO: VerifyAllAreUsed() throws an exception for any unconsumed args

        public IEnumerable<PositionalArgument> GetAllPositionals() => _positionals.Where(a => !a.Consumed);

        public IEnumerable<NamedArgument> GetAllNamed() => _nameds.Values.SelectMany(n => n).Where(a => !a.Consumed);

        public IEnumerable<FlagArgument> GetAllFlags() => _flags.Values.Where(a => !a.Consumed);

        // TODO: GetFlag(name) which returns the IArgument
        public bool HasFlag(string name, bool markConsumed = false)
        {
            name = name.ToLowerInvariant();
            if (!_flags.ContainsKey(name))
                return false;
            var flag = _flags[name];
            if (flag == null || flag.Consumed)
                return false;
            if (markConsumed)
                flag.MarkConsumed();
            return true;
        }

        // TODO: Method to create a sub-CommandArguments instance with some arguments added/removed

        public T MapTo<T>()
            where T : new() 
            => new CommandArgumentMapper().Map<T>(this);

        public IEnumerable<IArgument> GetAllArguments()
            => _positionals.Cast<IArgument>()
                .Concat(_nameds.SelectMany(kvp => kvp.Value))
                .Concat(_flags.Select(kvp => kvp.Value))
                .Where(p => !p.Consumed);

        public void ResetAllArguments()
        {
            foreach (var p in _positionals)
                p.MarkConsumed(false);
            foreach (var n in _nameds.Values.SelectMany(x => x))
                n.MarkConsumed(false);
            foreach (var f in _flags.Values)
                f.MarkConsumed(false);
        }
    }
}