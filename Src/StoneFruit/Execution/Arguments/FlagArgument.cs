namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A flag or switch argument with a name but no value
    /// </summary>
    public class FlagArgument : IParsedArgument
    {
        public FlagArgument(string name)
        {
            Name = name.ToLowerInvariant();
        }

        public string Name { get; }
    }

    public class FlagArgumentAccessor : IFlagArgument
    {
        public FlagArgumentAccessor(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Consumed { get; private set; }

        public IArgument MarkConsumed(bool consumed = true)
        {
            Consumed = consumed;
            return this;
        }

        public string AsString(string defaultValue = null) => true.ToString();

        public bool AsBool(bool defaultValue = false) => true;

        public int AsInt(int defaultValue = 0) => 1;

        public long AsLong(long defaultValue = 0) => 1L;
    }
}