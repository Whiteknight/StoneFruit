using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// A complete command with a verb and arguments
    /// </summary>
    public class CompleteCommand
    {
        public CompleteCommand(string verb, CommandArguments arguments)
        {
            Verb = verb;
            Arguments = arguments ?? CommandArguments.Empty();
        }

        public string Verb { get; }

        public CommandArguments Arguments { get; }

        // TODO: Should we keep track of other information? 
        // Complete command string?
        // Start time? / End time?
    }
}