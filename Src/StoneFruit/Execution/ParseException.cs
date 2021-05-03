using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Exception thrown by the argument parser or script parser when the input is not in a valid
    /// format.
    /// </summary>
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }

        protected ParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public ParseException()
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
