namespace StoneFruit.Execution.Arguments
{
    public class ParsedFlagPositionalOrNamedArgument : IParsedArgument
    {
        public ParsedFlagPositionalOrNamedArgument(string name, string value)
        {
            Name = name.ToLowerInvariant();
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}