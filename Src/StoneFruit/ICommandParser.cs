using System.Collections.Generic;
using StoneFruit.Execution;
using StoneFruit.Execution.Scripts;

namespace StoneFruit;

/// <summary>
/// Parser for commands. A Command object consists of a verb and a list of arguments.
/// </summary>
public interface ICommandParser
{
    /// <summary>
    /// Parse the line of text into a sequence of arguments. The first few arguments are
    /// expected to be the verb to invoke.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    IArguments ParseCommand(string command);

    Result<List<ArgumentsOrString>, ScriptsError> ParseScript(IReadOnlyList<string> lines, IArguments args);
}
