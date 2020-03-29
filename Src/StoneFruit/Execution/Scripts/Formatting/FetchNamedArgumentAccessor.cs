using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch an unconsumed named argument and return a new named argument with the same
    /// name and value
    /// </summary>
    public class FetchNamedArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;

        public FetchNamedArgumentAccessor(string name)
        {
            _name = name;
        }

        public IEnumerable<IArgument> Access(CommandArguments args)
        {
            var arg = args.Get(_name);
            if (!arg.Exists())
                return Enumerable.Empty<IArgument>();
            arg.MarkConsumed();
            return new [] { new NamedArgumentAccessor(_name, arg.AsString(string.Empty)) };
        }
    }
}