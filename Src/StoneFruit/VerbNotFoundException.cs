using System;
using System.Runtime.Serialization;

namespace StoneFruit
{
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