using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// A complete command with a verb and arguments
    /// </summary>
    public class Command
    {
        // No public constructor, we can only create one through the factory methods
        // below
        private Command()
        {
        }

        /// <summary>
        /// Create a new Command with given verb and arguments
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Command Create(string verb, IArguments arguments)
        {
            return new Command
            {
                Verb = verb,
                Alias = null,
                Raw = null,
                Arguments = arguments
            };
        }

        /// <summary>
        /// Create a new Command with given verb and arguments, and the raw parsed command
        /// string (usually only available from the parser)
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="arguments"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static Command CreateFromParser(string verb, IArguments arguments, string raw)
        {
            return new Command
            {
                Verb = verb,
                Alias = null,
                Raw = raw,
                Arguments = arguments
            };
        }

        /// <summary>
        /// The verb
        /// </summary>
        public string Verb { get; private set; }

        /// <summary>
        /// The user-input Alias, which is translated into the Verb
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The raw command string, if available. Null otherwise.
        /// </summary>
        public string Raw { get; private set; }

        /// <summary>
        /// The arguments
        /// </summary>
        public IArguments Arguments { get; private set; }

        /// <summary>
        /// Rename the verb. Sets the Alias to the old verb and sets the Verb to the new
        /// verb
        /// </summary>
        /// <param name="newVerb"></param>
        /// <returns></returns>
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