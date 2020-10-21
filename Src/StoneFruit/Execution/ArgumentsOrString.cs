using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Discriminated union for unparsed command strings or pre-parsed Command objects
    /// </summary>
    public class ArgumentsOrString
    {
        public ArgumentsOrString(string s)
        {
            Arguments = null;
            String = s;
        }

        public ArgumentsOrString(IArguments c)
        {
            Arguments = c;
            String = null;
        }

        public IArguments Arguments { get; }

        public string String { get; }

        public bool IsValid => !string.IsNullOrEmpty(String) || Arguments != null;

        public static implicit operator ArgumentsOrString(string s) => new ArgumentsOrString(s);
    }
}