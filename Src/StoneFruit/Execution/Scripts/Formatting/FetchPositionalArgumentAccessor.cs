using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Return a new positional argument if there is an unconsumed positional input at the
    /// given index
    /// </summary>
    public class FetchPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly int _index;

        public FetchPositionalArgumentAccessor(int index)
        {
            _index = index;
        }

        public IEnumerable<IArgument> Access(CommandArguments args)
        {
            var arg = args.Get(_index);
            if (!arg.Exists() || arg.Consumed)
                return Enumerable.Empty<IArgument>();
            arg.MarkConsumed();
            return new [] { new PositionalArgumentAccessor(arg.AsString(string.Empty)) };
        }
    }
}