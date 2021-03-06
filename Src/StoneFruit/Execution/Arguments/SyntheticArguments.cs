﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Args object built from pre-existing arguments which are unambiguous. This is an
    /// optimization over the ParsedArguments because we don't have to keep track of
    /// ambiguous cases.
    /// </summary>
    public class SyntheticArguments : IArguments
    {
        private readonly IReadOnlyList<IPositionalArgument> _positionals;
        private readonly IReadOnlyDictionary<string, List<INamedArgument>> _nameds;
        private readonly IReadOnlyDictionary<string, IFlagArgument> _flags;
        private int _accessedShiftIndex;

        public SyntheticArguments(IReadOnlyList<IArgument> arguments)
        {
            _positionals = arguments
                .OfType<IPositionalArgument>()
                .ToList();

            _nameds = arguments.OfType<INamedArgument>()
                .GroupBy(a => a.Name)
                .ToDictionary(g => g.Key.ToLowerInvariant(), g => g.ToList());

            _flags = arguments
                .OfType<IFlagArgument>()
                .ToDictionary(a => a.Name.ToLowerInvariant());

            _accessedShiftIndex = 0;
        }

        /// <summary>
        /// The raw, unparsed argument string if available
        /// </summary>
        public string Raw => string.Empty;

        /// <summary>
        /// Create an empty arguments object
        /// </summary>
        /// <returns></returns>
        public static SyntheticArguments Empty() => new SyntheticArguments(new IArgument[0]);

        /// <summary>
        /// Create named arguments from a list of name/value tuples
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static SyntheticArguments From(params (string, string)[] args)
        {
            var argsList = args
                .Select(t => new NamedArgument(t.Item1, t.Item2))
                .ToList();
            return new SyntheticArguments(argsList);
        }

        /// <summary>
        /// Create named arguments from a dictionary of name/value pairs
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static SyntheticArguments From(IReadOnlyDictionary<string, string> args)
        {
            var argsList = args
                .Select(kvp => new NamedArgument(kvp.Key, kvp.Value))
                .ToList();
            return new SyntheticArguments(argsList);
        }

        /// <summary>
        /// An arguments object for a single value
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static SyntheticArguments Single(string arg)
            => new SyntheticArguments(new IArgument[] { new PositionalArgument(arg) });

        public void VerifyAllAreConsumed()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Arguments were provided which were not consumed.");
            sb.AppendLine();
            var unconsumed = _positionals.Where(p => !p.Consumed)
                .Cast<IArgument>()
                .Concat(_nameds.SelectMany(kvp => kvp.Value).Where(n => !n.Consumed))
                .Concat(_flags.Values.Where(f => !f.Consumed))
                .ToList();
            if (unconsumed.Count == 0)
                return;

            foreach (var u in unconsumed)
            {
                var str = u switch
                {
                    PositionalArgument p => p.AsString(),
                    NamedArgument n => $"'{n.Name}' = {n.AsString()}",
                    FlagArgument f => $"flag {f.Name}",
                    _ => "Unknown"
                };
                sb.AppendLine(str);
            }

            throw new CommandArgumentException(sb.ToString());
        }

        public void ResetAllArguments()
        {
            foreach (var p in _positionals)
                p.MarkConsumed(false);
            foreach (var n in _nameds.Values.SelectMany(x => x))
                n.MarkConsumed(false);
            foreach (var f in _flags.Values)
                f.MarkConsumed(false);
        }

        public IPositionalArgument Shift()
        {
            if (_positionals.Count <= _accessedShiftIndex)
                return MissingArgument.NoPositionals();
            return _positionals[_accessedShiftIndex++];
        }

        public IPositionalArgument Get(int index)
        {
            if (index >= _positionals.Count)
                return MissingArgument.NoPositionals();

            if (_positionals[index].Consumed)
                return MissingArgument.PositionalConsumed(index);

            return _positionals[index];
        }

        public INamedArgument Get(string name)
        {
            name = name.ToLowerInvariant();
            if (!_nameds.ContainsKey(name))
                return MissingArgument.NoneNamed(name);

            var firstAvailable = _nameds[name].FirstOrDefault(a => !a.Consumed);
            if (firstAvailable == null)
                return MissingArgument.NoneNamed(name);

            return firstAvailable;
        }

        public IEnumerable<IPositionalArgument> GetAllPositionals()
            => _positionals.Where(a => !a.Consumed);

        public IEnumerable<IArgument> GetAll(string name)
        {
            name = name.ToLowerInvariant();
            if (!_nameds.ContainsKey(name))
                return Enumerable.Empty<IArgument>();

            return _nameds[name].Where(a => !a.Consumed);
        }

        public IEnumerable<INamedArgument> GetAllNamed()
            => _nameds.Values
                .SelectMany(n => n)
                .Where(a => !a.Consumed);

        public IFlagArgument GetFlag(string name)
        {
            name = name.ToLowerInvariant();
            if (!_flags.ContainsKey(name))
                return MissingArgument.FlagMissing(name);
            return _flags[name].Consumed ? MissingArgument.FlagConsumed(name) : _flags[name];
        }

        public bool HasFlag(string name, bool markConsumed = false)
            => _flags.ContainsKey(name.ToLowerInvariant());

        public IEnumerable<IFlagArgument> GetAllFlags() => _flags.Values.Where(a => !a.Consumed);
    }
}