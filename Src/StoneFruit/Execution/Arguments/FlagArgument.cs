namespace StoneFruit.Execution.Arguments
{
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
    }
}