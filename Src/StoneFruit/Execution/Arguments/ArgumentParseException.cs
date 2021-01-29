using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Exception thrown in response to errors encountered during argument access
    /// </summary>
    [Serializable]
    public class ArgumentParseException : Exception
    {
        public ArgumentParseException(string message)
            : base(message)
        {
        }

        protected ArgumentParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public ArgumentParseException() : base()
        {
        }

        public ArgumentParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static ArgumentParseException MissingRequiredArgument(int index)
            => new ArgumentParseException($"Required argument at position {index} was not provided");

        public static ArgumentParseException MissingRequiredArgument(string argument)
            => new ArgumentParseException($"Required argument named '{argument}' was not provided");
    }
}
