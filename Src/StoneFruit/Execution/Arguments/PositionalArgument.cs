namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Accessor for a positional argument with a value but no name
    /// </summary>
    public class PositionalArgument : IPositionalArgument
    {
        public PositionalArgument(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Consumed { get; set; }

        public string AsString(string defaultValue = null) => string.IsNullOrEmpty(Value) ? defaultValue : Value;

        public bool AsBool(bool defaultValue = false) => this.As(bool.Parse, defaultValue);

        public int AsInt(int defaultValue = 0) => this.As(int.Parse, defaultValue);

        public long AsLong(long defaultValue = 0) => this.As(long.Parse, defaultValue);
    }
}