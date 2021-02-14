using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Exception we throw during Engine execution, including inside built-in handlers
    /// </summary>
    [Serializable]
    public class ExecutionException : Exception
    {
        public ExecutionException()
        {
        }

        public ExecutionException(string message)
            : base(message)
        {
        }

        public ExecutionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
