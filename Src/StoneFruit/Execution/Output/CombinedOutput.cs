using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Output
{
    public class CombinedOutput : IOutput
    {
        private readonly IOutput _primary;
        private readonly IReadOnlyList<IOutput> _secondaries;

        public CombinedOutput(IOutput primary, IEnumerable<IOutput> secondaries)
        {
            _primary = primary;
            _secondaries = secondaries.OrEmptyIfNull().ToList();
        }

        public IOutput Color(Brush brush)
        {
            if (_primary == null)
                return this;
            return new CombinedOutput(_primary.Color(brush), _secondaries);
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
        {
            return _primary?.Prompt(prompt, mustProvide, keepHistory) ?? string.Empty;
        }
    }
}