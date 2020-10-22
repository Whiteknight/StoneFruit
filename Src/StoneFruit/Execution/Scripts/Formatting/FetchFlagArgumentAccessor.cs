using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch an unconsumed flag argument and return a new flag argument with the same name
    /// </summary>
    public class FetchFlagArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;
        private readonly string _newName;

        public FetchFlagArgumentAccessor(string name, string newName = null)
        {
            _name = name;
            _newName = newName;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var flag = args.GetFlag(_name);
            if (!flag.Exists())
                return Enumerable.Empty<IArgument>();
            flag.MarkConsumed();
            return new[] { new FlagArgument(_newName ?? _name) };
        }
    }
}