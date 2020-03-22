using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution.Arguments
{
    [Serializable]
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException(string message) 
            : base(message)
        {
        }

        protected CommandArgumentException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}