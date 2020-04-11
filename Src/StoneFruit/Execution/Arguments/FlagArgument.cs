namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Accessor for a flag argument which contains a name but no value
    /// </summary>
    public class FlagArgument : IFlagArgument
    {
        public FlagArgument(string name)
        {
            Name = name.ToLowerInvariant();
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