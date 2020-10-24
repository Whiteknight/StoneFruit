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
        public string Verb { get; }

        public VerbNotFoundException(string verb)
            : base($"Could not find handler for verb '{verb}'. Please check your spelling and try again")
        {
            Verb = verb;
        }

        protected VerbNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static VerbNotFoundException FromArguments(IArguments arguments)
        {
            var firstPositional = arguments.Shift();
            if (!firstPositional.Exists())
                return new VerbNotFoundException("No verb provided. You must provide at least one verb");
            return new VerbNotFoundException($"Could not find a handler for verb {firstPositional.AsString()}");
        }
    }
}