using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    public class DelimitedMultiPositionalArgument : IPositionalArgument
    {
        private readonly IReadOnlyList<IPositionalArgument> _args;

        private bool _consumed;

        public DelimitedMultiPositionalArgument(IReadOnlyList<IPositionalArgument> args)
        {
            _args = args;

            Value = string.Join(" ", _args.Select(a => a.AsString()));
        }

        public string Value { get; }

        public bool Consumed
        {
            get => _consumed;

            set
            {
                _consumed = value;
                foreach (var arg in _args)
                    arg.MarkConsumed(_consumed);
            }
        }

        public bool AsBool(bool defaultValue = false) => true;

        public int AsInt(int defaultValue = 0) => 0;

        public long AsLong(long defaultValue = 0) => 0L;

        public string AsString(string defaultValue = "") => Value;
    }
}
