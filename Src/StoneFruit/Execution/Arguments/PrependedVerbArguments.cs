using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Wrapper for IArguments which allows prepending a pre-parsed Verb to the front of the
    /// arguments.
    /// </summary>
    public class PrependedVerbArguments : IArguments, IVerbSource
    {
        private readonly IReadOnlyList<string> _verb;
        private readonly IArguments _inner;

        public PrependedVerbArguments(IEnumerable<string> verb, IArguments inner)
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(inner, nameof(inner));

            _verb = verb.ToList();
            _inner = inner;
        }

        public void SetVerbCount(int count)
        {
            // Do nothing, we don't actually care what this number is
        }

        public IReadOnlyList<IPositionalArgument> GetVerbCandidatePositionals()
            => _verb.Select(v => new PositionalArgument(v)).ToList();

        public string Raw => _inner.Raw;

        public IReadOnlyList<string> GetUnconsumed() => _inner.GetUnconsumed();

        public IPositionalArgument Get(int index) => _inner.Get(index);

        public INamedArgument Get(string name) => _inner.Get(name);

        public IEnumerable<INamedArgument> GetAllNamed(string name) => _inner.GetAllNamed(name);

        public IEnumerable<INamedArgument> GetAllNamed() => _inner.GetAllNamed();

        public IEnumerable<IFlagArgument> GetAllFlags() => _inner.GetAllFlags();

        public IEnumerable<IPositionalArgument> GetAllPositionals() => _inner.GetAllPositionals();

        public IFlagArgument GetFlag(string name) => _inner.GetFlag(name);

        public bool HasFlag(string name, bool markConsumed = false) => _inner.HasFlag(name, markConsumed);

        public void Reset() => _inner.Reset();

        public IPositionalArgument Shift() => _inner.Shift();
    }
}
