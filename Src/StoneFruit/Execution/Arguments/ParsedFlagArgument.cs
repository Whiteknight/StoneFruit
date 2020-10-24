namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A flag or switch argument with a name but no value. This is a raw parsed argument and
    /// will not be used in this way
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