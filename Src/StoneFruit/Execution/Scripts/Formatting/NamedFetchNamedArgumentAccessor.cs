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
        private readonly bool _required;
        private readonly string _defaultValue;

        public NamedFetchNamedArgumentAccessor(string newName, string oldName, bool required, string defaultValue)
        {
            _newName = newName;
            _oldName = oldName;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var arg = args.Get(_oldName);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return new[] { new NamedArgument(_newName, arg.AsString(string.Empty)), };
            }
            if (!_required)
                return Enumerable.Empty<IArgument>();
            if (!string.IsNullOrEmpty(_defaultValue))
                return new[] { new NamedArgument(_newName, _defaultValue) };
            throw ArgumentParseException.MissingRequiredArgument(_oldName);
        }
    }
}