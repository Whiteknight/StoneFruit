using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Return a new named argument with a given name and value
    /// </summary>
    public class LiteralNamedArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;
        private readonly string _value;

        public LiteralNamedArgumentAccessor(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public IEnumerable<IArgument> Access(IArguments args) 
            => new [] { new NamedArgumentAccessor(_name, _value) };
    }
}