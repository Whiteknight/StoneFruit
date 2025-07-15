using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts;

/// <summary>
/// A formatting object used to create a command from a list of arguments. The format
/// operation may use some or all of the input arguments, with possible modifications.
/// </summary>
public readonly record struct CommandFormat(IReadOnlyList<IArgumentAccessor> Arguments)
{
    // Notice that for a format like "verb <..args>" the "verb" is itself a positional argument
    // So the comandFormat isn't a template with literals and arguments, it's just a list of
    // arguments, some of which will be treated like verbs.
    public Result<ArgumentsOrString, ScriptsError> Format(IArguments args)
        => Arguments
            .Select(a => a.Access(args))
            .Aggregate((f, s) => f.Combine(s, (x, y) => [.. x, .. y], (e1, e2) => e1.Combine(e2)))
            .Bind(FilterEmptyLines)
            .Map(ToArguments);

    private static Result<IReadOnlyList<IArgument>, ScriptsError> FilterEmptyLines(IReadOnlyList<IArgument> args)
    {
        if (args.Count == 0)
            return new EmptyLine();
        return Result<IReadOnlyList<IArgument>, ScriptsError>.Create(args);
    }

    private static ArgumentsOrString ToArguments(IReadOnlyList<IArgument> s)
    {
        var args = s.ToSyntheticArguments();
        args.Reset();
        return new ArgumentsOrString(args);
    }
}
