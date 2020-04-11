namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A flag or switch argument with a name but no value
    /// </summary>
    public class ParsedFlagArgument : IParsedArgument
    {
        public ParsedFlagArgument(string name)
        {
            Name = name.ToLowerInvariant();
        }

        public string Name { get; }
    }
}