using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
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
