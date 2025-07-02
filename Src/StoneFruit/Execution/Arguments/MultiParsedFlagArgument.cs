using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A ParsedArgument which represents multiple flags, which have been specified together.
/// </summary>
public class MultiParsedFlagArgument : IParsedArgument
{
    public MultiParsedFlagArgument(IEnumerable<string> names)
    {
        Names = names.Select(n => n.ToLowerInvariant()).ToList();
    }

    public IReadOnlyList<string> Names { get; }

    public IEnumerable<IParsedArgument> ToIndividualArgs()
        => Names.Select(n => new ParsedFlagArgument(n));
}
