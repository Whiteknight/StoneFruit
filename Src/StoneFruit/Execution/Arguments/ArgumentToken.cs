using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments;

// ArgumentToken is an intermediate type between the arg parser and IArguments
// The parser outputs these tokens, and then they are converted to IArguments according to rules.
public abstract record ArgumentToken;
public record ParsedPositional(string Value) : ArgumentToken;
public record ParsedNamed(string Name, string Value) : ArgumentToken;
public record ParsedFlag(string Name) : ArgumentToken;
public record ParsedFlagAndPositionalOrNamed(string Name, string Value) : ArgumentToken;
public record ParsedMultiFlag(IReadOnlyList<string> Names) : ArgumentToken
{
    public IEnumerable<ArgumentToken> ToIndividualArgs()
        => Names.Select(n => new ParsedFlag(n));
}
