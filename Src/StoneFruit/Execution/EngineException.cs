using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
    [Serializable]
    public class EngineException : Exception
    {
        public EngineException()
        {
        }

        public EngineException(string message) : base(message)
        {
        }

        public EngineException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EngineException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static EngineException NoHandlers()
        {
            return new EngineException(@"No Handlers configured.

You must have at least one handler configured in order to start the engine.");
        }
    }
}