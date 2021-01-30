using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Exception thrown by the engine when a verb cannot be found.
    /// </summary>
    [Serializable]
    public class VerbNotFoundException : Exception
    {
        public string Verb { get; private set; }

        public VerbNotFoundException(string message)
            : base(message)
        {
            Verb = string.Empty;
        }

        protected VerbNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Verb = string.Empty;
        }

        public VerbNotFoundException() : base()
        {
            Verb = string.Empty;
        }

        public VerbNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            Verb = string.Empty;
        }

        public static VerbNotFoundException FromArguments(IArguments arguments)
        {
            var firstPositional = arguments.Shift();
            if (!firstPositional.Exists())
                return new VerbNotFoundException("No verb provided. You must provide at least one verb");
            return new VerbNotFoundException($"Could not find a handler for verb {firstPositional.AsString()}") { Verb = firstPositional.AsString() };
        }
    }
}
