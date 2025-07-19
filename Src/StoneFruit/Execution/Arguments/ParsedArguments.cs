using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Provides access to a collection of IArgument objects by name or position. Arguments
/// from the parser are inherently ambiguous, so this collection keeps them in a raw
/// state until intent is determined by user access.
/// </summary>
public class ParsedArguments : IArgumentCollection, IVerbSource
{
    private readonly List<RawArg> _rawArguments;
    private readonly List<IPositionalArgument> _accessedPositionals;
    private readonly Dictionary<string, List<INamedArgument>> _accessedNameds;
    private readonly Dictionary<string, IFlagArgument> _accessedFlags;

    private int _lastRawPositionalIndex;

    public ParsedArguments(IEnumerable<ArgumentToken> arguments, string? rawArgs = null)
    {
        Raw = rawArgs ?? string.Empty;
        _accessedPositionals = [];
        _accessedNameds = new Dictionary<string, List<INamedArgument>>(StringComparer.OrdinalIgnoreCase);
        _accessedFlags = new Dictionary<string, IFlagArgument>(StringComparer.OrdinalIgnoreCase);
        _rawArguments = arguments
            .SelectMany(a => a switch
            {
                ParsedMultiFlag mp => mp.ToIndividualArgs(),
                _ => [a]
            })
            .Select(a => new RawArg(a))
            .ToList();
    }

    private enum AccessType
    {
        Unaccessed,
        Accessed,
        AccessedAsFlag,
        AccessedAsPositional
    }

    private sealed class RawArg
    {
        public RawArg(ArgumentToken arg)
        {
            Argument = arg;
            Access = AccessType.Unaccessed;
        }

        public ArgumentToken Argument { get; }

        public AccessType Access { get; set; }
    }

    public string Raw { get; }

    // We access verbs before we access any args, so we can work entirely out of the
    // raw unprocessed args list here.
    public IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals()
    {
        var candidates = new List<IPositionalArgument>();
        for (int i = 0; i < _rawArguments.Count; i++)
        {
            var arg = _rawArguments[i];
            if (arg.Access == AccessType.Accessed || arg.Access == AccessType.AccessedAsPositional)
                continue;

            // In some cases we're going to double-convert an arg. We'll pull more positionals
            // than we need, decide that some of them are not part of the verb, and put back
            // the rest. It's a small price to pay to avoid the complexity of conditionally
            // caching them.
            if (arg.Argument is ParsedPositional pa)
                candidates.Add(new PositionalArgument(pa.Value));
        }

        return candidates;
    }

    public void SetVerbCount(int count)
    {
        // TODO: See if we can avoid removing the raw args from the beginning, so .Reset()
        // actually returns us to the beginning state?
        _rawArguments.RemoveRange(0, count);
    }

    public IReadOnlyList<string> GetUnconsumed()
    {
        // We need to find all raw arguments which are not accessed, or all accessed
        // arguments which are marked unconsumed.

        var fromRaw = _rawArguments
            .Where(raw => raw.Access != AccessType.Accessed)
            .Select(raw => raw.Argument switch
            {
                ParsedPositional p => p.Value,
                ParsedNamed n => $"'{n.Name}' = {n.Value}",
                ParsedFlag f => $"flag {f.Name}",
                ParsedFlagAndPositionalOrNamed fp => raw.Access switch
                {
                    AccessType.AccessedAsFlag => fp.Value,
                    AccessType.AccessedAsPositional => $"flag {fp.Name}",
                    _ => $"'{fp.Name}', {fp.Value}"
                },
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

    public void Reset()
    {
        _accessedPositionals.Clear();
        _accessedNameds.Clear();
        _accessedFlags.Clear();

        foreach (var raw in _rawArguments)
            raw.Access = AccessType.Unaccessed;
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

    public INamedArgument Get(string name, bool caseSensitive = true)
    {
        // Check the already-accessed named args. If we have it, return it.

        var found = _accessedNameds
            .Where(kvp => Equals(kvp.Key, name, caseSensitive))
            .SelectMany(kvp => kvp.Value)
            .FirstOrDefault(n => !n.Consumed);
        if (found != null)
            return found;

        // Loop through all unaccessed args looking for the first one with the given
        // name.
        return AccessNamedUntil(n => Equals(n, name, caseSensitive), () => true)
            .GetValueOrDefault(MissingArgument.NoneNamed(name));
    }

    public IEnumerable<IPositionalArgument> GetAllPositionals()
    {
        AccessPositionalsUntil(() => false);
        return _accessedPositionals.Where(a => !a.Consumed);
    }

    public IEnumerable<INamedArgument> GetAllNamed(string name, bool caseSensitive = true)
    {
        name = name.ToLowerInvariant();
        AccessNamedUntil(n => Equals(n, name, caseSensitive), () => false);
        return _accessedNameds.TryGetValue(name, out var value)
            ? value.Where(a => !a.Consumed)
            : [];
    }

    public IEnumerable<INamedArgument> GetAllNamed()
    {
        // Access all named arguments
        AccessNamedUntil(_ => true, () => false);
        return _accessedNameds.Values
            .SelectMany(n => n)
            .Where(a => !a.Consumed);
    }

    public IFlagArgument GetFlag(string name, bool caseSensitive = true)
    {
        // Check if we've already accessed this flag. If so, return it if unconsumed
        // or not found
        var found = _accessedFlags
            .Where(kvp => !kvp.Value.Consumed && Equals(kvp.Key, name, caseSensitive))
            .Select(kvp => kvp.Value)
            .FirstOrDefault();
        if (found != null)
            return found;

        // Loop through unaccessed args looking for a matching flag.
        return AccessFlagsUntil(name, n => Equals(n, name, caseSensitive), () => true);
    }

    public bool HasFlag(string name, bool markConsumed = false, bool caseSensitive = true)
        => _accessedFlags.ContainsKey(name)
            || AccessFlagsUntil(name, n => Equals(n, name, caseSensitive), () => true).Exists();

    public IEnumerable<IFlagArgument> GetAllFlags()
    {
        AccessFlagsUntil("", _ => true, () => false);
        return _accessedFlags.Values.Where(a => !a.Consumed);
    }

    // Loop over all unconsumed raw positional arguments, accessing each one until a condition is satisfied.
    // When the condition is matched, return the current item.
    private IPositionalArgument AccessPositionalsUntil(Func<bool> match)
    {
        for (; _lastRawPositionalIndex < _rawArguments.Count; _lastRawPositionalIndex++)
        {
            var i = _lastRawPositionalIndex;
            var arg = _rawArguments[i];
            if (arg.Access == AccessType.Accessed || arg.Access == AccessType.AccessedAsPositional)
                continue;

            IPositionalArgument? accessor = arg.Argument switch
            {
                ParsedPositional pa => MarkAccessedAsPositional(i, pa),
                ParsedFlagAndPositionalOrNamed fp => MarkAccessedAsPositional(arg, fp),
                _ => null
            };

            if (accessor != null && match())
                return accessor;
        }

        return MissingArgument.NoPositionals();
    }

    private static bool Equals(string s1, string s2, bool caseSensitive)
        => s1.Equals(s2, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

    private PositionalArgument MarkAccessedAsPositional(RawArg arg, ParsedFlagAndPositionalOrNamed fp)
    {
        var accessor = new PositionalArgument(fp.Value);
        _accessedPositionals.Add(accessor);

        // If we have already consumed the flag portion, mark the whole arg consumed
        // otherwise, mark that we have only consumed the positional portion.
        arg.Access = arg.Access == AccessType.AccessedAsFlag
            ? AccessType.Accessed
            : AccessType.AccessedAsPositional;
        return accessor;
    }

    private PositionalArgument MarkAccessedAsPositional(int i, ParsedPositional pa)
    {
        var accessor = new PositionalArgument(pa.Value);
        _rawArguments[i].Access = AccessType.Accessed;
        _accessedPositionals.Add(accessor);
        return accessor;
    }

    private Maybe<INamedArgument> AccessNamedUntil(Func<string, bool> shouldAccess, Func<bool> isComplete)
    {
        for (int i = 0; i < _rawArguments.Count; i++)
        {
            var accessor = GetNamedAccessorForArgument(i, shouldAccess);
            if (accessor.IsSuccess && isComplete())
                return accessor.GetValueOrThrow();
        }

        return default;
    }

    private Maybe<NamedArgument> GetNamedAccessorForArgument(int i, Func<string, bool> shouldAccess)
    {
        var arg = _rawArguments[i];

        // Any consumption at all means it can't be a named value
        if (arg.Access != AccessType.Unaccessed)
            return default;

        if (arg.Argument is ParsedNamed n && shouldAccess(n.Name))
            return MarkAccessedAsNamed(i, n);

        if (arg.Argument is ParsedFlagAndPositionalOrNamed n2 && shouldAccess(n2.Name))
            return MarkAccessedAsNamed(i, n2);

        // For all other argument types, there is no named accessor
        return default;
    }

    private NamedArgument MarkAccessedAsNamed(int i, ParsedFlagAndPositionalOrNamed n2)
    {
        var accessor = new NamedArgument(n2.Name, n2.Value);
        _rawArguments[i].Access = AccessType.Accessed;
        AccessNamed(accessor);
        return accessor;
    }

    private NamedArgument MarkAccessedAsNamed(int i, ParsedNamed n)
    {
        var accessor = new NamedArgument(n.Name, n.Value);
        _rawArguments[i].Access = AccessType.Accessed;
        AccessNamed(accessor);
        return accessor;
    }

    private void AccessNamed(NamedArgument n)
    {
        if (!_accessedNameds.ContainsKey(n.Name))
            _accessedNameds.Add(n.Name, []);
        _accessedNameds[n.Name].Add(n);
    }

    private IFlagArgument AccessFlagsUntil(string name, Func<string, bool> isMatch, Func<bool> isComplete)
    {
        for (int i = 0; i < _rawArguments.Count; i++)
        {
            var arg = _rawArguments[i];
            if (arg.Access == AccessType.Accessed || arg.Access == AccessType.AccessedAsFlag)
                continue;

            IFlagArgument? accessor = null;

            if (arg.Argument is ParsedFlag f && isMatch(f.Name))
                accessor = MarkAccessedAsFlag(i, f);

            if (arg.Argument is ParsedFlagAndPositionalOrNamed fp && isMatch(fp.Name))
                accessor = MarkAccessedAsFlag(arg, fp);

            if (accessor != null && isComplete())
                return accessor;
        }

        return MissingArgument.FlagMissing(name);
    }

    private FlagArgument MarkAccessedAsFlag(int i, ParsedFlag f)
    {
        var accessor = new FlagArgument(f.Name);
        _rawArguments[i].Access = AccessType.Accessed;
        _accessedFlags.Add(f.Name, accessor);
        return accessor;
    }

    private FlagArgument MarkAccessedAsFlag(RawArg arg, ParsedFlagAndPositionalOrNamed fp)
    {
        var accessor = new FlagArgument(fp.Name);
        _accessedFlags.Add(fp.Name, accessor);
        arg.Access = arg.Access == AccessType.AccessedAsPositional
            ? AccessType.Accessed
            : AccessType.AccessedAsFlag;
        return accessor;
    }
}
