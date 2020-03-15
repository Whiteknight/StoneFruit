using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution.Arguments
{
    [Serializable]
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException()
        {
        }

        public CommandArgumentException(string message) 
            : base(message)
        {
        }

        public CommandArgumentException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        protected CommandArgumentException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}