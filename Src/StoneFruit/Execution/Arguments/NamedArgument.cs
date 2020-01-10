namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument defined by a name
    /// </summary>
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