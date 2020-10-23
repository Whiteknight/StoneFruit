using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Return a new positional argument if there is an unconsumed positional input at the
    /// given index
    /// </summary>
    public class FetchPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly int _index;
        private readonly bool _required;
        private readonly string _defaultValue;

        public FetchPositionalArgumentAccessor(int index, bool required, string defaultValue)
        {
            _index = index;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var arg = args.Get(_index);
            if (arg.Exists() && !arg.Consumed)
            {
                arg.MarkConsumed();
                return new[] { new PositionalArgument(arg.AsString(string.Empty)) };
            }
            if (!_required)
                return Enumerable.Empty<IArgument>();
            if (!string.IsNullOrEmpty(_defaultValue))
                return new[] { new PositionalArgument(_defaultValue) };

            throw ArgumentParseException.MissingRequiredArgument(_index);
        }
    }
}