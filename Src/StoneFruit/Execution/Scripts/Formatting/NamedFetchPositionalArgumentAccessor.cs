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
        private readonly bool _required;
        private readonly string _defaultValue;

        public NamedFetchPositionalArgumentAccessor(string newName, int index, bool required, string defaultValue)
        {
            _newName = newName;
            _index = index;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var arg = args.Get(_index);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return new[] { new NamedArgument(_newName, arg.AsString(string.Empty)) };
            }
            if (!_required)
                return Enumerable.Empty<IArgument>();
            if (!string.IsNullOrEmpty(_defaultValue))
                return new[] { new NamedArgument(_newName, _defaultValue) };
            throw ArgumentParseException.MissingRequiredArgument(_index);
        }
    }
}