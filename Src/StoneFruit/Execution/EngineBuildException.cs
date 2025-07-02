using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Exception we throw during engine build-up if conflicting or invalid settings have been
    /// selected.
    /// </summary>
    [Serializable]
    public class EngineBuildException : Exception
    {
        public EngineBuildException(string message) : base(message)
        {
        }

        public EngineBuildException()
        {
        }

        public EngineBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static EngineBuildException NoHandlers()
        {
            return new EngineBuildException(@"No Handlers configured.

You must have at least one handler configured in order to start the engine.");
        }
    }
}
