namespace StoneFruit.Execution.Arguments
{
    public class NamedArgument : IArgument
    {
        public NamedArgument(string name, string value)
        {
            Name = name;
            Value = value;
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