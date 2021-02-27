using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Provides access to a collection of IArgument objects by name or position. Arguments
    /// from the parser are inherently ambiguous, so this collection keeps them in a raw
    /// state until intent is determined by user access. At this point the arguments are
    /// moved to an "accessed" state where they are known unambiguously.
    /// </summary>
    public class ParsedArguments : IArguments, IVerbSource
    {
        private readonly List<IParsedArgument?> _rawArguments;
        private readonly List<IPositionalArgument> _accessedPositionals;
        private readonly Dictionary<string, List<INamedArgument>> _accessedNameds;
        private readonly Dictionary<string, IFlagArgument> _accessedFlags;

        private int _lastRawPositionalIndex;

        public ParsedArguments(IEnumerable<IParsedArgument> arguments, string? rawArgs = null)
        {
            Raw = rawArgs ?? string.Empty;
            _accessedPositionals = new List<IPositionalArgument>();
            _accessedNameds = new Dictionary<string, List<INamedArgument>>();
            _accessedFlags = new Dictionary<string, IFlagArgument>();
            _rawArguments = arguments
                .SelectMany(a => a switch
                {
                    MultiParsedFlagArgument mp => mp.ToIndividualArgs(),
                    _ => new[] { a }
                })
                .Cast<IParsedArgument?>()
                .ToList();
        }

        public string Raw { get; }

        public IReadOnlyList<string> Unconsumed
        {
            get
            {
                var fromRaw = _rawArguments
                .Where(a => a != null)
                .Select(u => u switch
                {
                    ParsedPositionalArgument p => p.Value,
                    ParsedNamedArgument n => $"'{n.Name}' = {n.Value}",
                    ParsedFlagArgument f => $"flag {f.Name}",
                    ParsedFlagPositionalOrNamedArgument fp => $"'{fp.Name}', {fp.Value}",
                    _ => "Unknown"
                });

                var fromAccessed = _accessedPositionals.Cast<IArgument>()
                    .Concat(_accessedNameds.SelectMany(kvp => kvp.Value))
                    .Concat(_accessedFlags.Values)
                    .Where(p => !p.Consumed)
                    .Select(u => u switch
                    {
                        IPositionalArgument p => p.Value,
                        INamedArgument n => $"'{n.Name}' = {n.Value}",
                        IFlagArgument f => $"flag {f.Name}",
                        _ => "Unknown"
                    });

                return fromRaw.Concat(fromAccessed).ToList();
            }
        }

        public void ResetAllArguments()
        {
            foreach (var p in _accessedPositionals)
                p.MarkConsumed(false);
            foreach (var n in _accessedNameds.Values.SelectMany(x => x))
                n.MarkConsumed(false);
            foreach (var f in _accessedFlags.Values)
                f.MarkConsumed(false);
        }

        public IPositionalArgument Shift() => AccessPositionalsUntil(() => true);

        public IPositionalArgument Get(int index)
        {
            // Check if we have accessed this index already. If so we either return it or
            // it's consumed already
            if (index < _accessedPositionals.Count)
                return !_accessedPositionals[index].Consumed ? _accessedPositionals[index] : MissingArgument.PositionalConsumed(index);

            // Loop through the unaccessed args until we either find the one we're looking
            // for or we run out.
            AccessPositionalsUntil(() => index < _accessedPositionals.Count);
            return index < _accessedPositionals.Count ? _accessedPositionals[index] : MissingArgument.NoPositionals();
        }

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
            var match = AccessNamedUntil(n => n == name, () => true);
            return match ?? MissingArgument.NoneNamed(name);
        }

        public IEnumerable<IPositionalArgument> GetAllPositionals()
        {
            AccessPositionalsUntil(() => false);
            return _accessedPositionals.Where(a => !a.Consumed);
        }

        // Loop over all raw positional arguments, accessing each one until a condition is satisfied.
        // When the condition is matched, return the current item.
        private IPositionalArgument AccessPositionalsUntil(Func<bool> match)
        {
            for (; _lastRawPositionalIndex < _rawArguments.Count; _lastRawPositionalIndex++)
            {
                var i = _lastRawPositionalIndex;
                var arg = _rawArguments[i];
                if (arg is ParsedPositionalArgument pa)
                {
                    var accessor = new PositionalArgument(pa.Value);
                    _rawArguments[i] = null;
                    _accessedPositionals.Add(accessor);
                    if (match())
                        return accessor;
                }

                if (arg is ParsedFlagPositionalOrNamedArgument fp)
                {
                    var accessor = new PositionalArgument(fp.Value);
                    _accessedPositionals.Add(accessor);
                    // Replace the Flag+Positional arg with just a flag, the positional is consumed
                    _rawArguments[i] = new ParsedFlagArgument(fp.Name);
                    if (match())
                        return accessor;
                }
            }

            return MissingArgument.NoPositionals();
        }

        public IEnumerable<IArgument> GetAll(string name)
        {
            name = name.ToLowerInvariant();
            AccessNamedUntil(n => n == name, () => false);
            return _accessedNameds.ContainsKey(name) ? _accessedNameds[name].Where(a => !a.Consumed) : Enumerable.Empty<IArgument>();
        }

        public IEnumerable<INamedArgument> GetAllNamed()
        {
            // Access all named arguments
            AccessNamedUntil(_ => true, () => false);
            return _accessedNameds.Values
                .SelectMany(n => n)
                .Where(a => !a.Consumed);
        }

        private INamedArgument? AccessNamedUntil(Func<string, bool> shouldAccess, Func<bool> isComplete)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var accessor = GetNamedAccessorForArgument(i, shouldAccess);
                if (accessor != null && isComplete())
                    return accessor;
            }

            return null;
        }

        private NamedArgument? GetNamedAccessorForArgument(int i, Func<string, bool> shouldAccess)
        {
            var arg = _rawArguments[i];
            if (arg is ParsedNamedArgument n && shouldAccess(n.Name))
            {
                var accessor = new NamedArgument(n.Name, n.Value);
                _rawArguments[i] = null;
                AccessNamed(accessor);
                return accessor;
            }

            if (arg is ParsedFlagPositionalOrNamedArgument n2 && shouldAccess(n2.Name))
            {
                var accessor = new NamedArgument(n2.Name, n2.Value);
                _rawArguments[i] = null;
                AccessNamed(accessor);
                return accessor;
            }

            // For all other argument types, there is no named accessor
            return null;
        }

        private void AccessNamed(NamedArgument n)
        {
            if (!_accessedNameds.ContainsKey(n.Name))
                _accessedNameds.Add(n.Name, new List<INamedArgument>());
            _accessedNameds[n.Name].Add(n);
        }

        public IFlagArgument GetFlag(string name)
        {
            name = name.ToLowerInvariant();

            // Check if we've already accessed this flag. If so, return it if unconsumed
            // or not found
            if (_accessedFlags.ContainsKey(name))
                return _accessedFlags[name].Consumed ? MissingArgument.FlagConsumed(name) : _accessedFlags[name];

            // Loop through unaccessed args looking for a matching flag.
            var match = AccessFlagsUntil(n => n == name, () => true);
            return match ?? MissingArgument.FlagMissing(name);
        }

        public bool HasFlag(string name, bool markConsumed = false)
        {
            name = name.ToLowerInvariant();
            if (_accessedFlags.ContainsKey(name))
                return true;
            var arg = AccessFlagsUntil(n => n == name, () => true);
            return arg?.Exists() == true;
        }

        public IEnumerable<IFlagArgument> GetAllFlags()
        {
            AccessFlagsUntil(_ => true, () => false);
            return _accessedFlags.Values.Where(a => !a.Consumed);
        }

        private IFlagArgument? AccessFlagsUntil(Func<string, bool> isMatch, Func<bool> isComplete)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var accessor = GetFlagAccessorForArgument(i, isMatch);
                if (accessor != null && isComplete())
                    return accessor;
            }

            return null;
        }

        private FlagArgument? GetFlagAccessorForArgument(int i, Func<string, bool> isMatch)
        {
            var arg = _rawArguments[i];
            if (arg is ParsedFlagArgument f && isMatch(f.Name))
            {
                var accessor = new FlagArgument(f.Name);
                _rawArguments[i] = null;
                _accessedFlags.Add(f.Name, accessor);
                return accessor;
            }

            if (arg is ParsedFlagPositionalOrNamedArgument fp && isMatch(fp.Name))
            {
                var accessor = new FlagArgument(fp.Name);
                _accessedFlags.Add(fp.Name, accessor);
                _rawArguments[i] = new ParsedPositionalArgument(fp.Value);
                return accessor;
            }

            return null;
        }

        // We access verbs before we access any args, so we can work entirely out of the
        // raw unprocessed args list here.
        public IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals()
        {
            var candidates = new List<IPositionalArgument>();
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                // In some cases we're going to double-convert an arg. We'll pull more positionals
                // than we need, decide that some of them are not part of the verb, and put back
                // the rest. It's a small price to pay to avoid the complexity of conditionally
                // caching them.
                if (_rawArguments[i] is ParsedPositionalArgument pa)
                    candidates.Add(new PositionalArgument(pa.Value));
            }
            return candidates;
        }

        public void SetVerbCount(int count)
        {
            _rawArguments.RemoveRange(0, count);
        }
    }
}
