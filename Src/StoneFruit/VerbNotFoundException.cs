using System;
using System.Runtime.Serialization;

namespace StoneFruit
{
    // TODO: Move this to a more appropriate namespace
    /// <summary>
    /// Exception thrown by the engine when a verb cannot be found. 
    /// </summary>
    [Serializable]
    public class VerbNotFoundException : Exception
    {
        public VerbNotFoundException()
        {
        }

        public VerbNotFoundException(string message) 
            : base($"Could not find handler for verb '{message}'. Please check your spelling and try again")
        {
        }

        public VerbNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected VerbNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}