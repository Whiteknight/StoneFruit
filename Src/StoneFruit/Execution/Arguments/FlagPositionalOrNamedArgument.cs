namespace StoneFruit.Execution.Arguments
{
    public class FlagPositionalOrNamedArgument : IParsedArgument
    {
        public FlagPositionalOrNamedArgument(string name, string value)
        {
            Name = name.ToLowerInvariant();
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}