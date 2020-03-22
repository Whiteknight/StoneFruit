using System;
using System.Runtime.Serialization;

namespace StoneFruit
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
    }
}