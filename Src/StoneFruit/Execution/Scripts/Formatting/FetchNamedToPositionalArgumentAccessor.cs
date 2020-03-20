using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch the value of an unconsumed named argument from the input and return that as a
    /// positional argument
    /// </summary>
    public class FetchNamedToPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;

        public FetchNamedToPositionalArgumentAccessor(string name)
        {
            _name = name;
        }

        public IEnumerable<IArgument> Access(CommandArguments args)
        {
            var arg = args.Get(_name);
            if (!arg.Exists())
                return Enumerable.Empty<IArgument>();
            arg.MarkConsumed();
            return new [] { new PositionalArgument(arg.AsString(string.Empty)) };
        }
    }
}