using System.Collections.Generic;

namespace StoneFruit.Execution.Scripts;

/// <summary>
/// Accessor to pull an argument out of IArguments based on some criteria.
/// </summary>
public interface IArgumentAccessor
{
    /// <summary>
    /// Given a set of arguments input to a script, return zero or more new arguments
    /// to pass to the scripted command.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    Result<IReadOnlyList<IArgument>, ScriptsError> Access(IArgumentCollection args, int line);
}
