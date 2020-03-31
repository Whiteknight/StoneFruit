using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// A complete command with a verb and arguments
    /// </summary>
    public class Command
    {
        private Command()
        {
        }

        public static Command Create(string verb, ICommandArguments arguments)
        {
            return new Command
            {
                Verb = verb,
                Alias = null,
                Raw = null,
                Arguments = arguments
            };
        }

        public static Command CreateFromParser(string verb, ICommandArguments arguments, string raw)
        {
            return new Command
            {
                Verb = verb,
                Alias = null,
                Raw = raw,
                Arguments = arguments
            };
        }

        public string Verb { get; private set; }
        public string Alias { get; private set; }
        public string Raw { get; private set; }

        public ICommandArguments Arguments { get; private set; }

        public Command Rename(string newVerb)
        {
            return new Command
            {
                Alias = Verb,
                Verb = newVerb,
                Arguments = Arguments,
                Raw = Raw
            };
        }
    }
}