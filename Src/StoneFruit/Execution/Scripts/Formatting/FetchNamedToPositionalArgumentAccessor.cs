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
        private readonly bool _required;
        private readonly string _defaultValue;

        public FetchNamedToPositionalArgumentAccessor(string name, bool required, string defaultValue)
        {
            _name = name;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var arg = args.Get(_name);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return new[] { new PositionalArgument(arg.AsString(string.Empty)) };
            }
            if (!_required)
                return Enumerable.Empty<IArgument>();
            if (!string.IsNullOrEmpty(_defaultValue))
                return new[] { new PositionalArgument(_defaultValue) };

            throw ArgumentParseException.MissingRequiredArgument(_name);
        }
    }
}