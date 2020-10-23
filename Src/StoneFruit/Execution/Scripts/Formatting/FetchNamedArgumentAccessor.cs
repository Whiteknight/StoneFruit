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
        private readonly bool _required;
        private readonly string _defaultValue;

        public FetchNamedArgumentAccessor(string name, bool required, string defaultValue)
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
                return new[] { new NamedArgument(_name, arg.AsString(string.Empty)) };
            }
            if (!_required)
                return Enumerable.Empty<IArgument>();
            if (!string.IsNullOrEmpty(_defaultValue))
                return new[] { new NamedArgument(_name, _defaultValue) };
            throw ArgumentParseException.MissingRequiredArgument(_name);
        }
    }
}