namespace StoneFruit.Execution.Arguments
{
    public class PositionalArgumentAccessor : IPositionalArgument
    {
        public PositionalArgumentAccessor(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Consumed { get; private set; }

        public IArgument MarkConsumed(bool consumed = true)
        {
            Consumed = consumed;
            return this;
        }

        public string AsString(string defaultValue = null) => string.IsNullOrEmpty(Value) ? defaultValue : Value;

        public bool AsBool(bool defaultValue = false) => this.As(bool.Parse, defaultValue);

        public int AsInt(int defaultValue = 0) => this.As(int.Parse, defaultValue);

        public long AsLong(long defaultValue = 0) => this.As(long.Parse, defaultValue);
    }
}