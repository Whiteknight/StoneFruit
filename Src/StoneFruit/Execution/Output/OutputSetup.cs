using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Execution.Output
{
    public class OutputSetup : IOutputSetup, ISetupBuildable<IOutput>
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
            var output = Build();
            services.AddSingleton(output);
        }

        public IOutput Build()
        {
            if (!_useConsole)
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
