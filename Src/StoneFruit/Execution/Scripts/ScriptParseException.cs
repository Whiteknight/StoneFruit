using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution.Scripts
{
    [Serializable]
    public class ScriptParseException : Exception
    {
        public ScriptParseException()
        {
        }

        public ScriptParseException(string message) : base(message)
        {
        }

        public ScriptParseException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ScriptParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}