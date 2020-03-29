using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch an unconsumed positional argument from the input and return it as the value
    /// of a named argument with a given name
    /// </summary>
    public class NamedFetchPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly string _newName;
        private readonly int _index;

        public NamedFetchPositionalArgumentAccessor(string newName, int index)
        {
            _newName = newName;
            _index = index;
        }

        public IEnumerable<IArgument> Access(CommandArguments args)
        {
            var arg = args.Get(_index);
            if (!arg.Exists())
                return Enumerable.Empty<IArgument>();
            arg.MarkConsumed();
            return new[] { new NamedArgumentAccessor(_newName, arg.AsString(string.Empty)),  };
        }
    }
}