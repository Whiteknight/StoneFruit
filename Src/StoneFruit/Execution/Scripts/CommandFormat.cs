using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting;

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

    public IArguments Format(IArguments args)
    {
        var argList = _args
            .SelectMany(a => a.Access(args))
            .Where(a => a != null)
            .ToList();
        return new SyntheticArguments(argList);
    }
}
