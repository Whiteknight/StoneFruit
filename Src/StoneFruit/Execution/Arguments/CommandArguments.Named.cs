using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Arguments
{
    public partial class CommandArguments
    {
        private readonly Dictionary<string, List<INamedArgument>> _accessedNameds;

        public INamedArgument Consume(string name) => Get(name).MarkConsumed() as INamedArgument;

        public INamedArgument Get(string name)
        {
            name = name.ToLowerInvariant();

            // Check the already-accessed named args. If we have it, return it.
            if (_accessedNameds.ContainsKey(name))
            {
                var firstAvailable = _accessedNameds[name].FirstOrDefault(a => !a.Consumed);
                if (firstAvailable != null)
                    return _accessedNameds[name].First();
            }

            // Loop through all unaccessed args looking for the first one with the given
            // name. 
            var match = AccessNamedUntil(n => n == name, () => true);
            return match ?? MissingArgument.NoneNamed(name);
        }

        public IEnumerable<IArgument> GetAll(string name)
        {
            name = name.ToLowerInvariant();
            AccessNamedUntil(n => n == name, () => false);
            if (_accessedNameds.ContainsKey(name))
                return _accessedNameds[name].Where(a => !a.Consumed);
            return Enumerable.Empty<IArgument>();
        }

        public IEnumerable<INamedArgument> GetAllNamed()
        {
            // Access all named arguments
            AccessNamedUntil(n => true, () => false);
            return _accessedNameds.Values
                .SelectMany(n => n)
                .Where(a => !a.Consumed);
        }

        private INamedArgument AccessNamedUntil(Func<string, bool> shouldAccess, Func<bool> isComplete)
        {
            for (int i = 0; i < _rawArguments.Count; i++)
            {
                var arg = _rawArguments[i];
                if (arg is NamedArgument n)
                {
                    if (!shouldAccess(n.Name))
                        continue;
                    var accessor = new NamedArgumentAccessor(n.Name, n.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                    if (isComplete())
                        return accessor;
                }

                if (arg is FlagPositionalOrNamedArgument n2)
                {
                    if (!shouldAccess(n2.Name))
                        continue;
                    var accessor = new NamedArgumentAccessor(n2.Name, n2.Value);
                    _rawArguments[i] = null;
                    AccessNamed(accessor);
                    if (isComplete())
                        return accessor;
                }
            }

            return null;
        }

        private void AccessNamed(NamedArgumentAccessor n)
        {
            if (!_accessedNameds.ContainsKey(n.Name))
                _accessedNameds.Add(n.Name, new List<INamedArgument>());
            _accessedNameds[n.Name].Add(n);
        }
    }
}
