using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments
{
    public class DictionaryEnvironmentFactory : IEnvironmentFactory
    {
        private readonly IReadOnlyDictionary<string, object> _environments;

        public DictionaryEnvironmentFactory(IReadOnlyDictionary<string, object> environments)
        {
            _environments = environments;
        }

        public object Create(string name) 
            => _environments.ContainsKey(name) ? _environments[name] : null;

        public IReadOnlyCollection<string> ValidEnvironments => _environments.Keys.ToList();
    }
}
