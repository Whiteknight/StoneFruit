namespace StoneFruit.Execution
{
    // simple discriminated union type, sometimes commands come pre-parsed
    public class CommandObjectOrString
    {
        public Command Object { get; private set; }
        public string String { get; private set; }

        // TODO: We can clean up some code with some implicit cast operators

        public static CommandObjectOrString FromObject(Command command)
        {
            return new CommandObjectOrString { Object = command, String = null };
        }

        public static CommandObjectOrString FromString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            return new CommandObjectOrString { Object = null, String = s };
        }
    }
}