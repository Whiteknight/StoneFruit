using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch an unconsumed named argument value, and return a new named argument with the
    /// fetched value but a given name
    /// </summary>
    public class NamedFetchNamedArgumentAccessor : IArgumentAccessor
    {
        private readonly string _newName;
        private readonly string _oldName;

        public NamedFetchNamedArgumentAccessor(string newName, string oldName)
        {
            _newName = newName;
            _oldName = oldName;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var arg = args.Get(_oldName);
            if (!arg.Exists())
                return Enumerable.Empty<IArgument>();
            arg.MarkConsumed();
            return new [] { new NamedArgument(_newName, arg.AsString(string.Empty)),  };
        }
    }
}