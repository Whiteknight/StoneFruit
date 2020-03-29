using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Return a new positional argument with a given value
    /// </summary>
    public class LiteralPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly string _value;

        public LiteralPositionalArgumentAccessor(string value)
        {
            _value = value;
        }

        public IEnumerable<IArgument> Access(CommandArguments args) 
            => new [] { new PositionalArgumentAccessor(_value) };
    }
}