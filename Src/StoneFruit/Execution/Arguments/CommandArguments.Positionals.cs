using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    public partial class CommandArguments
    {
        private readonly List<IPositionalArgument> _accessedPositionals;

        /// <summary>
        /// Get the next positional value
        /// </summary>
        /// <returns></returns>
        public IPositionalArgument Shift()
        {
            // CommandArguments can be constructed with un-accessed IParsedArgument or with
            // pre-accessed IArgument. In the former case, pull the next un-accessed item 
            // off the raw list. In the later, we need to keep an index into the accessed
            // list and return from there.
            if (_accessedShiftIndex < 0)
                return AccessPositionalsUntil(() => true) ?? MissingArgument.NoPositionals();

            if (_accessedPositionals.Count >= _accessedShiftIndex)
                return MissingArgument.NoPositionals();
            return _accessedPositionals[_accessedShiftIndex++];
        }

        public IPositionalArgument Consume(int index) => Get(index).MarkConsumed() as IPositionalArgument;

        public IPositionalArgument Get(int index)
        {
            // Check if we have accessed this index already. If so we either return it or
            // it's consumed already
            if (index < _accessedPositionals.Count)
            {
                if (_accessedPositionals[index].Consumed)
                    return MissingArgument.PositionalConsumed(index);
                return _accessedPositionals[index];
            }

            // Loop through the unaccessed args until we either find the one we're looking
            // for or we run out.
            AccessPositionalsUntil(() => index < _accessedPositionals.Count);
            if (index >= _accessedPositionals.Count)
                return MissingArgument.NoPositionals();
            return _accessedPositionals[index];
        }

        public IEnumerable<IPositionalArgument> GetAllPositionals()
        {
            AccessPositionalsUntil(() => false);
            return _accessedPositionals.Where(a => !a.Consumed);
        }

        // Loop over all raw positional arguments, accessing each one until a condition is satisfied.
        // When the condition is matched, return the current item.
        private IPositionalArgument AccessPositionalsUntil(Func<bool> match)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is PositionalArgument pa)
                {
                    var accessor = new PositionalArgumentAccessor(pa.Value);
                    _rawArguments[i] = null;
                    _accessedPositionals.Add(accessor);
                    if (match())
                        return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    var accessor = new PositionalArgumentAccessor(fp.Value);
                    _accessedPositionals.Add(accessor);
                    // Replace the Flag+Positional arg with just a flag, the positional is consumed
                    var flag = new FlagArgument(fp.Name);
                    _rawArguments[i] = flag;
                    if (match())
                        return accessor;
                }
            }

            return null;
        }
    }
}
