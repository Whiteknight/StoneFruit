using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments;

public abstract record ParsedArgument;
public record ParsedPositional(string Value) : ParsedArgument;
public record ParsedNamed(string Name, string Value) : ParsedArgument;
public record ParsedFlag(string Name) : ParsedArgument;
public record ParsedFlagAndPositionalOrNamed(string Name, string Value) : ParsedArgument;
public record ParsedMultiFlag(IReadOnlyList<string> Names) : ParsedArgument
{
    public IEnumerable<ParsedArgument> ToIndividualArgs()
        => Names.Select(n => new ParsedFlag(n));
}
