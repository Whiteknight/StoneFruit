using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts.Formatting;

namespace StoneFruit
{
    /// <summary>
    /// Parser for commands. A Command object consists of a verb and a list of arguments.
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// Parse the line of text into a Command with a verb and arguments
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        Command ParseCommand(string line);

        /// <summary>
        /// Parse the line of text into an IArguments object
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IArguments ParseArguments(string args);

        /// <summary>
        /// Parse the line of text into a CommandFormat template
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        CommandFormat ParseScript(string script);
    }
}