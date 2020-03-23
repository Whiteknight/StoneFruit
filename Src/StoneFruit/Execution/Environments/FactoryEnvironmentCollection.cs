using System.Collections.Generic;
using System.Linq;

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
        private readonly Dictionary<int, string> _nameIndices;
        private readonly HashSet<string> _validNames;

        public FactoryEnvironmentCollection(IEnvironmentFactory environmentFactory)
        {
            var environments = environmentFactory.ValidEnvironments;
            if (environments == null || environments.Count == 0)
                environments = new[] { Constants.EnvironmentNameDefault };
            _environmentFactory = environmentFactory;
            _namedCache = new Dictionary<string, object>();
            _nameIndices = environments
                .Select((item, index) => new { Index = index + 1, Item = item })
                .ToDictionary(x => x.Index, x => x.Item);
            _validNames = new HashSet<string>(environments);
        }

        public object Current { get; private set; }

        public string CurrentName { get; private set; }

        public IReadOnlyDictionary<int, string> GetNames() => _nameIndices;

        public string GetName(int index) => _nameIndices.ContainsKey(index) ? _nameIndices[index] : null;

        public object Get(string name)
        {
            if (_namedCache.ContainsKey(name))
                return _namedCache[name];
            if (!_validNames.Contains(name))
                return default;
            var env = _environmentFactory.Create(name);
            _namedCache.Add(name, env);
            return env;
        }

        public object Get(int idx) => _nameIndices.ContainsKey(idx) ? Get(_nameIndices[idx]) : default;

        public bool IsValid(string name) => _validNames.Contains(name);

        public bool IsValid(int index) => _nameIndices.ContainsKey(index);

        public void SetCurrent(string name)
        {
            CurrentName = name;
            Current = Get(name);
        }

        public void SetCurrent(int index)
        {
            if (_nameIndices.ContainsKey(index))
                SetCurrent(_nameIndices[index]);
        }
    }
}