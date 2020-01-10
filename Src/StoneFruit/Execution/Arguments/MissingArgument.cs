namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A Null Object implementation of IArgument
    /// </summary>
    public class MissingArgument : IArgument
    {
        public string Message { get; }

        public MissingArgument(string message)
        {
            Message = message;
        }

        public string Value => null;

        public bool Consumed => true;

        public IArgument MarkConsumed() => this;

        public void Throw() => throw new CommandArgumentException(Message);
    }
}