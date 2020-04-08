using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Execution
{
    // simple discriminated union type, sometimes commands come pre-parsed
    public class CommandOrString
    {
        private CommandOrString(string s)
        {
            Object = null;
            String = s;
        }

        private CommandOrString(Command c)
        {
            Object = c;
            String = null;
        }

        public Command Object { get; }

        public string String { get; }

        public bool IsValid => !string.IsNullOrEmpty(String) || Object != null;

        public static implicit operator CommandOrString(string s) 
            => new CommandOrString(s);

        public static implicit operator CommandOrString(Command command)
            => new CommandOrString(command);
    }
}