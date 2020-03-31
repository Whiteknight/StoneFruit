namespace StoneFruit.Execution.Arguments
{
    public class FlagArgumentAccessor : IFlagArgument
    {
        public FlagArgumentAccessor(string name)
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