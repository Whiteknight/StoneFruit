namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A flag or switch argument with a name but no value
    /// </summary>
    public class FlagArgument : IArgument
    {
        public FlagArgument(string name)
        {
            Name = name;
            Value = "";
        }

        public string Name { get; }

        public string Value { get; }

        public bool Consumed { get; private set; }

        public IArgument MarkConsumed()
        {
            Consumed = true;
            return this;
        }

        public string AsString(string defaultValue = null) => true.ToString();

        public bool AsBool(bool defaultValue = false) => true;

        public int AsInt(int defaultValue = 0) => 1;

        public long AsLong(long defaultValue = 0) => 1L;
    }
}