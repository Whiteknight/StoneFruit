using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Args object built from pre-existing arguments which are unambiguous. This is an
    /// optimization over the ParsedArguments because we don't have to keep track of
    /// ambiguous cases.
    /// </summary>
    public class SyntheticArguments : IArguments, IVerbSource
    {
        private readonly IReadOnlyList<IPositionalArgument> _positionals;
        private readonly IReadOnlyDictionary<string, List<INamedArgument>> _nameds;
        private readonly IReadOnlyDictionary<string, IFlagArgument> _flags;
        private int _verbCount;
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
            _verbCount = 0;
        }

        public string Raw => string.Empty;

        public IReadOnlyList<string> GetUnconsumed()
            => _positionals
                .Skip(_verbCount)
                .Where(p => !p.Consumed)
                .Cast<IArgument>()
                .Concat(_nameds.SelectMany(kvp => kvp.Value).Where(n => !n.Consumed))
                .Concat(_flags.Values.Where(f => !f.Consumed))
                .Select(a => a switch
                {
                    PositionalArgument p => p.AsString(),
                    NamedArgument n => $"'{n.Name}' = {n.AsString()}",
                    FlagArgument f => $"flag {f.Name}",
                    _ => "Unknown"
                })
                .ToList();

        /// <summary>
        /// Create an empty arguments object
        /// </summary>
        /// <returns></returns>
        public static SyntheticArguments Empty() => new SyntheticArguments(Array.Empty<IArgument>());

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
        /// Create from a list of strings to be treated as positional arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static SyntheticArguments From(params string[] args)
        {
            var argsList = args
                .Select(s => new PositionalArgument(s))
                .ToList();
            return new SyntheticArguments(argsList);
        }

        public void Reset()
        {
            foreach (var p in _positionals.Skip(_verbCount))
                p.MarkConsumed(false);
            foreach (var n in _nameds.Values.SelectMany(x => x))
                n.MarkConsumed(false);
            foreach (var f in _flags.Values)
                f.MarkConsumed(false);
        }

        public IPositionalArgument Shift()
        {
            var currentIndex = _verbCount + _accessedShiftIndex;
            if (_positionals.Count <= currentIndex)
                return MissingArgument.NoPositionals();
            _accessedShiftIndex++;
            return _positionals[currentIndex];
        }

        public IPositionalArgument Get(int index)
        {
            var realIndex = index + _verbCount;
            if (realIndex >= _positionals.Count)
                return MissingArgument.NoPositionals();

            if (_positionals[realIndex].Consumed)
                return MissingArgument.PositionalConsumed(index);

            return _positionals[realIndex];
        }

        public INamedArgument Get(string name)
        {
            name = name.ToLowerInvariant();
            if (!_nameds.ContainsKey(name))
                return MissingArgument.NoneNamed(name);
            return _nameds[name].FirstOrDefault(a => !a.Consumed) ?? MissingArgument.NoneNamed(name);
        }

        public IEnumerable<IPositionalArgument> GetAllPositionals()
            => _positionals.Skip(_verbCount).Where(a => !a.Consumed);

        public IEnumerable<INamedArgument> GetAllNamed(string name)
        {
            name = name.ToLowerInvariant();
            return _nameds.ContainsKey(name) ? _nameds[name].Where(a => !a.Consumed) : Enumerable.Empty<INamedArgument>();
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

        public IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals() => _positionals;

        public void SetVerbCount(int count)
        {
            _verbCount = count;
        }
    }
}
