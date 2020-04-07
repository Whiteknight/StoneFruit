using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// An IEnvironmentCollection implementation which uses an IEnvironmentFactory to create environments
    /// on demand
    /// </summary>
    public class FactoryEnvironmentCollection : IEnvironmentCollection
    {
        private readonly IEnvironmentFactory _environmentFactory;
        private readonly Dictionary<string, object> _namedCache;
        private readonly IReadOnlyList<string> _nameList;
        private readonly HashSet<string> _validNames;

        public FactoryEnvironmentCollection(IEnvironmentFactory environmentFactory)
        {
            Assert.ArgumentNotNull(environmentFactory, nameof(environmentFactory));
            var environments = environmentFactory.ValidEnvironments;
            if (environments == null || environments.Count == 0)
                environments = new[] { Constants.EnvironmentNameDefault };
            _environmentFactory = environmentFactory;
            _namedCache = new Dictionary<string, object>();
            _nameList = environments.ToList();
            _validNames = new HashSet<string>(environments);
        }

        public object Current { get; private set; }

        public string CurrentName { get; private set; }

        public IReadOnlyList<string> GetNames() => _nameList;

        public bool IsValid(string name)
        {
            if (name == null)
                return false;
            return _validNames.Contains(name);
        }

        public void SetCurrent(string name)
        {
            if (name == null)
                return;
            CurrentName = name;
            Current = Get(name);
        }

        private object Get(string name)
        {
            if (name == null)
                return default;
            if (_namedCache.ContainsKey(name))
                return _namedCache[name];
            if (!_validNames.Contains(name))
                return default;
            var env = _environmentFactory.Create(name);
            _namedCache.Add(name, env);
            return env;
        }
    }
}