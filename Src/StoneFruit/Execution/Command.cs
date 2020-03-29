using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// A complete command with a verb and arguments
    /// </summary>
    public class Command
    {
        public Command(string verb, CommandArguments arguments, string raw = null)
        {
            Verb = verb;
            Raw = raw;
            Arguments = arguments ?? CommandArguments.Empty();
        }

        public string Verb { get; }
        public string Raw { get; }

        public CommandArguments Arguments { get; }
    }
}