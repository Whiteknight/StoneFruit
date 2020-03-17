using System.Collections.Generic;

namespace StoneFruit.Execution.Output
{
    public class OutputSetup : IOutputSetup
    {
        private readonly List<IOutput> _secondaries;

        public OutputSetup()
        {
            _secondaries = new List<IOutput>();
        }

        public IOutputSetup Add(IOutput output)
        {
            if (output != null)
                _secondaries.Add(output);
            return this;
        }

        public IOutput Build()
        {
            if (_secondaries.Count == 0)
                return new ConsoleOutput();
            return new CombinedOutput(new ConsoleOutput(), _secondaries);
        }
    }
}
