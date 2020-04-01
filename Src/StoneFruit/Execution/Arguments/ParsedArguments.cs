using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Provides access to a list of IArgument objects but name or position.
    /// </summary>
    public class ParsedArguments : IArguments
    {
        private readonly List<IParsedArgument> _rawArguments;
        private readonly List<IPositionalArgument> _accessedPositionals;
        private readonly Dictionary<string, List<INamedArgument>> _accessedNameds;
        private readonly Dictionary<string, IFlagArgument> _accessedFlags;

        // TODO: Would like to do some book-keeping to avoid multiple traversals of the _raw list
        // TODO: Would like to clean the _raw list of nulls regularly

        // Empty args object with no values in it
        public ParsedArguments()
        {
            Raw = string.Empty;
            _accessedPositionals = new List<IPositionalArgument>();
            _accessedNameds = new Dictionary<string, List<INamedArgument>>();
            _accessedFlags = new Dictionary<string, IFlagArgument>();
            _rawArguments = new List<IParsedArgument>();
        }

        // Args object built from parsed objects, which aren't accessed yet
        public ParsedArguments(IEnumerable<IParsedArgument> arguments)
            : this(string.Empty, arguments)
        {
        }

        // Args object built from parsed objects, which aren't accessed yet
        public ParsedArguments(string rawArgs, IEnumerable<IParsedArgument> arguments)
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

        public void VerifyAllAreConsumed()
        {
            var fromRaw = _rawArguments
                .Where(a => a != null)
                .Select(u => u switch
                {
                    PositionalArgument p => p.Value,
                    NamedArgument n => $"'{n.Name}' = {n.Value}",
                    FlagArgument f => $"flag {f.Name}",
                    FlagPositionalOrNamedArgument fp => $"'{fp.Name}', {fp.Value}",
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

            var fromAll = fromRaw.Concat(fromAccessed).ToList();
            if (!fromAll.Any())
                return;

            var sb = new StringBuilder();
            sb.AppendLine("Arguments were provided which were not consumed.");
            sb.AppendLine();
            foreach (var line in fromAll)
                sb.AppendLine(line);
            throw new CommandArgumentException(sb.ToString());
        }

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

        /// <summary>
        /// Get the next positional value
        /// </summary>
        /// <returns></returns>
        public IPositionalArgument Shift() 
            => AccessPositionalsUntil(() => true) ?? MissingArgument.NoPositionals();

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
            AccessPositionalsUntil(() => index < _accessedPositionals.Count);
            if (index >= _accessedPositionals.Count)
                return MissingArgument.NoPositionals();
            return _accessedPositionals[index];
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
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is PositionalArgument pa)
                {
                    var accessor = new PositionalArgumentAccessor(pa.Value);
                    _rawArguments[i] = null;
                    _accessedPositionals.Add(accessor);
                    if (match())
                        return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    var accessor = new PositionalArgumentAccessor(fp.Value);
                    _accessedPositionals.Add(accessor);
                    // Replace the Flag+Positional arg with just a flag, the positional is consumed
                    var flag = new FlagArgument(fp.Name);
                    _rawArguments[i] = flag;
                    if (match())
                        return accessor;
                }
            }

            return null;
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

        public IEnumerable<IArgument> GetAll(string name)
        {
            name = name.ToLowerInvariant();
            AccessNamedUntil(n => n == name, () => false);
            if (_accessedNameds.ContainsKey(name))
                return _accessedNameds[name].Where(a => !a.Consumed);
            return Enumerable.Empty<IArgument>();
        }

        public IEnumerable<INamedArgument> GetAllNamed()
        {
            // Access all named arguments
            AccessNamedUntil(n => true, () => false);
            return _accessedNameds.Values
                .SelectMany(n => n)
                .Where(a => !a.Consumed);
        }

        private INamedArgument AccessNamedUntil(Func<string, bool> shouldAccess, Func<bool> isComplete)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is NamedArgument n)
                {
                    if (!shouldAccess(n.Name))
                        continue;
                    var accessor = new NamedArgumentAccessor(n.Name, n.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                    if (isComplete())
                        return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument n2)
                {
                    if (!shouldAccess(n2.Name))
                        continue;
                    var accessor = new NamedArgumentAccessor(n2.Name, n2.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                    if (isComplete())
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
            return arg.Exists();
        }

        public IEnumerable<IFlagArgument> GetAllFlags()
        {
            AccessFlagsUntil(n => true, () => false);
            return _accessedFlags.Values.Where(a => !a.Consumed);
        }

        private IFlagArgument AccessFlagsUntil(Func<string, bool> isMatch, Func<bool> isComplete)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is FlagArgument f)
                {
                    if (!isMatch(f.Name))
                        continue;
                    var accessor = new FlagArgumentAccessor(f.Name);
                    _rawArguments[i] = null;
                    _accessedFlags.Add(f.Name, accessor);
                    if (isComplete())
                        return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    if (!isMatch(fp.Name))
                        continue;
                    var accessor = new FlagArgumentAccessor(fp.Name);
                    _accessedFlags.Add(fp.Name, accessor);
                    _rawArguments[i] = new PositionalArgument(fp.Value);
                    if (isComplete())
                        return accessor;
                }
            }

            return null;
        }
    }
}