using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments;

// ArgumentToken is an intermediate type between the arg parser and IArguments
// The parser outputs these tokens, and then they are converted to IArguments according to rules.
public interface IArgumentToken;

public record ParsedPositional(string Value) : IArgumentToken;

public record ParsedNamed(string Name, string Value) : IArgumentToken;

public record ParsedFlag(string Name) : IArgumentToken;

public record ParsedFlagAndPositionalOrNamed(string Name, string Value) : IArgumentToken;

public record ParsedMultiFlag(IReadOnlyList<string> Names) : IArgumentToken
{
    public IEnumerable<IArgumentToken> ToIndividualArgs()
        => Names.Select(n => new ParsedFlag(n));
}
