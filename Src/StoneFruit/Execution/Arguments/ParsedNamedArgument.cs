namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument defined by a name
    /// </summary>
    public class ParsedNamedArgument : IParsedArgument
    {
        public ParsedNamedArgument(string name, string value)
        {
            Name = name.ToLowerInvariant();
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}