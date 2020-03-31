namespace StoneFruit.Execution.Arguments
{
    public class NamedArgumentAccessor : INamedArgument
    {
        public NamedArgumentAccessor(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        public bool Consumed { get; private set; }

        public IArgument MarkConsumed(bool consumed = true)
        {
            Consumed = consumed;
            return this;
        }

        public string AsString(string defaultValue = null) => Value ?? defaultValue;

        public bool AsBool(bool defaultValue = false) => this.As(bool.Parse, defaultValue);

        public int AsInt(int defaultValue = 0) => this.As(int.Parse, defaultValue);

        public long AsLong(long defaultValue = 0) => this.As(long.Parse, defaultValue);
    }
}