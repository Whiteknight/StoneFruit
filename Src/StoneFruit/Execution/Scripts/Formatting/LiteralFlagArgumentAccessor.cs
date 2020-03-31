using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Return a new flag argument with a given name
    /// </summary>
    public class LiteralFlagArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;

        public LiteralFlagArgumentAccessor(string name)
        {
            _name = name;
        }

        public IEnumerable<IArgument> Access(IArguments args) 
            => new [] { new FlagArgumentAccessor(_name) };
    }
}