using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// Environment factory which can return entries from a dictionary
    /// </summary>
    public class DictionaryEnvironmentFactory : IEnvironmentFactory
    {
        private readonly IReadOnlyDictionary<string, object> _environments;

        public DictionaryEnvironmentFactory(IReadOnlyDictionary<string, object> environments)
        {
            _environments = environments;
            ValidEnvironments = _environments.Keys.ToList();
        }

        public object Create(string name)
            => _environments.ContainsKey(name) ? _environments[name] : null;

        public IReadOnlyCollection<string> ValidEnvironments { get; }
    }
}
