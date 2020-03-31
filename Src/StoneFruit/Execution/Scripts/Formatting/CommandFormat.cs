using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// A formatting object used to create a command from a list of arguments. The format
    /// operation may use some or all of the input arguments, with possible modifications.
    /// </summary>
    public class CommandFormat
    {
        private readonly string _verb;
        private readonly IReadOnlyList<IArgumentAccessor> _args;

        public CommandFormat(string verb, IReadOnlyList<IArgumentAccessor> args)
        {
            _verb = verb;
            _args = args;
        }

        public Command Format(IArguments args)
        {
            var argsList = _args
                .SelectMany(a => a.Access(args))
                .Where(a => a != null)
                .ToList();
            var commandArguments = new SyntheticArguments(argsList);
            return Command.Create(_verb, commandArguments);
        }
    }
}