using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// A formatting object used to create a command from a list of arguments. The format
    /// operation may use some or all of the input arguments, with possible modifications.
    /// </summary>
    public class CommandFormat
    {
        private readonly IReadOnlyList<IArgumentAccessor> _args;

        public CommandFormat(IReadOnlyList<IArgumentAccessor> args)
        {
            _args = args;
        }

        public IArguments Format(IArguments args)
        {
            var argList = new List<IArgument>();
            foreach (var a in _args)
            {
                foreach (var newArg in a.Access(args))
                {
                    if (newArg != null)
                        argList.Add(newArg);
                }
                args.ResetAllArguments();
            }
            return new SyntheticArguments(argList);
        }
    }
}
