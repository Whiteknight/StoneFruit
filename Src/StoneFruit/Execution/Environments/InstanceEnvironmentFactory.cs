using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// An IEnvironmentFactory implementation which always returns exactly one instance
    /// </summary>
    public class InstanceEnvironmentFactory : IEnvironmentFactory
    {
        private readonly object _instance;

        public InstanceEnvironmentFactory(object instance)
        {
            _instance = instance;
        }

        public object Create(string name)
        {
            return _instance;
        }

        public IReadOnlyCollection<string> ValidEnvironments => new[] { "" };
    }
}