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

        public FetchFlagArgumentAccessor(string name)
        {
            _name = name;
        }

        public IEnumerable<IArgument> Access(CommandArguments args)
        {
            var flag = args.GetFlag(_name);
            if (!flag.Exists())
                return Enumerable.Empty<IArgument>();
            flag.MarkConsumed();
            return new [] { new FlagArgument(_name) };
        }
    }
}