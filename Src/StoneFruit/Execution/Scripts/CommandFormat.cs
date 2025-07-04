using System.Collections.Generic;
using System.Linq;
using StoneFruit;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts;

/// <summary>
/// A formatting object used to create a command from a list of arguments. The format
/// operation may use some or all of the input arguments, with possible modifications.
/// </summary>
public class CommandFormat
{
    private readonly IReadOnlyList<IArgumentAccessor> _args;

    public CommandFormat(IReadOnlyList<IArgumentAccessor> args)
    {
        _args = args;
    }

    // Notice that for a format like "verb <..args>" the "verb" is itself a positional argument
    // So the comandFormat isn't a template with literals and arguments, it's just a list of
    // arguments, some of which will be treated like verbs.
    public IArguments Format(IArguments args)
        => _args
            .SelectMany(a => a.Access(args))
            .Where(a => a != null)
            .ToSyntheticArguments();
}
