namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument which is defined by it's order in the list, not by name
    /// </summary>
    public class PositionalArgument : IArgument
    {
        public PositionalArgument(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Consumed { get; private set; }

        public IArgument MarkConsumed()
        {
            Consumed = true;
            return this;
        }

        public string AsString(string defaultValue = null) => string.IsNullOrEmpty(Value) ? defaultValue : Value;

        public bool AsBool(bool defaultValue = false) => this.As(bool.Parse, defaultValue);

        public int AsInt(int defaultValue = 0) => this.As(int.Parse, defaultValue);

        public long AsLong(long defaultValue = 0) => this.As(long.Parse, defaultValue);
    }
}