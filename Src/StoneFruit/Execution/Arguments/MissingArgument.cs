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

        public string AsString(string defaultValue = null) => defaultValue;

        public bool AsBool(bool defaultValue = false) => defaultValue;

        public int AsInt(int defaultValue = 0) => defaultValue;

        public long AsLong(long defaultValue = 0) => defaultValue;

        public void Throw() => throw new CommandArgumentException(Message);
    }
}