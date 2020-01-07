namespace StoneFruit.Execution.Arguments
{
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
    }
}