using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Exception thrown in response to errors encountered during argument access
    /// </summary>
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