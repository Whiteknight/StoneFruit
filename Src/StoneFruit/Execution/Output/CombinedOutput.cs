using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Output
{
    public class CombinedOutput : IOutput
    {
        private readonly IOutput _primary;
        private readonly IReadOnlyList<IOutput> _secondaries;

        public CombinedOutput(IEnumerable<IOutput> secondaries)
        {
            _primary = new NullOutput();
            _secondaries = secondaries.OrEmptyIfNull().ToList();
        }

        public CombinedOutput(IOutput primary, IEnumerable<IOutput> secondaries)
        {
            _primary = primary;
            _secondaries = secondaries.OrEmptyIfNull().ToList();
        }

        public IOutput Color(Func<Brush, Brush> changeBrush)
        {
            if (_primary == null || changeBrush == null)
                return this;
            var newPrimary = _primary.Color(changeBrush);
            if (!ReferenceEquals(newPrimary, _primary))
                return new CombinedOutput(newPrimary, _secondaries);
            return this;
        }

        public IOutput WriteLine()
        {
            _primary?.WriteLine();
            foreach (var s in _secondaries)
                s.WriteLine();
            return this;
        }

        public IOutput WriteLine(string line)
        {
            _primary?.WriteLine(line);
            foreach (var s in _secondaries)
                s.WriteLine(line);
            return this;
        }

        public IOutput Write(string str)
        {
            _primary?.Write(str);
            foreach (var s in _secondaries)
                s.Write(str);
            return this;
        }

        public string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
            => _primary?.Prompt(prompt, mustProvide, keepHistory) ?? string.Empty;
    }
}
