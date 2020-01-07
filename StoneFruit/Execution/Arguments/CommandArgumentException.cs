using System;
using System.Runtime.Serialization;

namespace StoneFruit.Execution.Arguments
{
    [Serializable]
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException()
        {
        }

        public CommandArgumentException(string message) : base(message)
        {
        }

        public CommandArgumentException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CommandArgumentException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static CommandArgumentException InvalidIndex(int index)
        {
            return new CommandArgumentException($"Requested positional argument {index} not found. Too few arguments were passed");
        }

        public static CommandArgumentException InvalidName(string name)
        {
            return new CommandArgumentException($"No arguments provided with requested name '{name}'");
        }
    }
}