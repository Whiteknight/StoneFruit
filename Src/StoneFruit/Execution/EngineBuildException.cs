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
        public EngineBuildException(string message) : base(message)
        {
        }

        protected EngineBuildException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static EngineBuildException NoHandlers()
        {
            return new EngineBuildException(@"No Handlers configured.

You must have at least one handler configured in order to start the engine.");
        }
    }
}