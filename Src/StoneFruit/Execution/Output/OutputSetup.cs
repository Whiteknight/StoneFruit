using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Execution.Output
{
    public class OutputSetup : IOutputSetup
    {
        private bool _useConsole;
        private readonly List<IOutput> _secondaries;

        public OutputSetup()
        {
            _useConsole = true;
            _secondaries = new List<IOutput>();
        }

        public IOutputSetup DoNotUseConsole()
        {
            _useConsole = false;
            return this;
        }

        public IOutputSetup Add(IOutput output)
        {
            if (output != null)
                _secondaries.Add(output);
            return this;
        }

        public void BuildUp(IServiceCollection services)
        {
            var output = GetConfiguredOutput();
            services.AddSingleton(output);
        }

        private IOutput GetConfiguredOutput()
        {
            if (_useConsole == false)
            {
                // This seems like a problem
                if (_secondaries.Count == 0)
                    return new NullOutput();
                if (_secondaries.Count == 1)
                    return _secondaries.First();
                return new CombinedOutput(null, _secondaries);
            }

            if (_secondaries.Count == 0)
                return new ConsoleOutput();
            return new CombinedOutput(new ConsoleOutput(), _secondaries);
        }
    }
}
