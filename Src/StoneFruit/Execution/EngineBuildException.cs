using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Exception we throw during engine build-up
    /// </summary>
    [Serializable]
    public class EngineBuildException : Exception
    {
        public EngineBuildException()
        {
        }

        public EngineBuildException(string message) : base(message)
        {
        }

        public EngineBuildException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EngineBuildException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
