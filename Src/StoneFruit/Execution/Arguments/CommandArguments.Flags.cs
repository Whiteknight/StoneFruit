using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    public partial class CommandArguments
    {
        private readonly Dictionary<string, IFlagArgument> _accessedFlags;

        public IFlagArgument GetFlag(string name)
        {
            name = name.ToLowerInvariant();

            // Check if we've already accessed this flag. If so, return it if unconsumed
            // or not found
            if (_accessedFlags.ContainsKey(name))
                return _accessedFlags[name].Consumed ? MissingArgument.FlagConsumed(name) : _accessedFlags[name];

            // Loop through unaccessed args looking for a matching flag.
            var match = AccessFlagsUntil(n => n == name, () => true);
            return match ?? MissingArgument.FlagMissing(name);
        }

        public IFlagArgument ConsumeFlag(string name) => GetFlag(name).MarkConsumed() as IFlagArgument;

        public bool HasFlag(string name, bool markConsumed = false)
        {
            var flag = GetFlag(name);
            if (!flag.Exists())
                return false;
            if (markConsumed)
                flag.MarkConsumed();
            return true;
        }

        public IEnumerable<IFlagArgument> GetAllFlags()
        {
            AccessFlagsUntil(n => true, () => false);
            return _accessedFlags.Values.Where(a => !a.Consumed);
        }

        private IFlagArgument AccessFlagsUntil(Func<string, bool> isMatch, Func<bool> isComplete)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is FlagArgument f)
                {
                    if (!isMatch(f.Name))
                        continue;
                    var accessor = new FlagArgumentAccessor(f.Name);
                    _rawArguments[i] = null;
                    _accessedFlags.Add(f.Name, accessor);
                    if (isComplete())
                        return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument fp)
                {
                    if (!isMatch(fp.Name))
                        continue;
                    var accessor = new FlagArgumentAccessor(fp.Name);
                    _accessedFlags.Add(fp.Name, accessor);
                    _rawArguments[i] = new PositionalArgument(fp.Value);
                    if (isComplete())
                        return accessor;
                }
            }

            return null;
        }
    }
}
