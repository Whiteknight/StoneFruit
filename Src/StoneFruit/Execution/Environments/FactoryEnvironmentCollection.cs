using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// An IEnvironmentCollection implementation which uses an IEnvironmentFactory to create environments
    /// on demand.
    /// </summary>
    public class FactoryEnvironmentCollection : IEnvironmentCollection
    {
        private readonly IEnvironmentFactory _environmentFactory;
        private readonly Dictionary<string, object> _namedCache;
        private readonly IReadOnlyList<string> _nameList;
        private readonly HashSet<string> _validNames;

        private IResult<object> _current;
        private IResult<string> _currentName;

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
            _current = FailureResult<object>.Instance;
            _currentName = FailureResult<string>.Instance;
        }

        public IResult<object> GetCurrent() => _current;

        public IResult<string> GetCurrentName() => _currentName;

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
            var result = Get(name);
            if (!result.HasValue)
                throw new InvalidOperationException($"Environment {name} does not exist");
            _currentName = Result.Success(name);
            _current = result;
        }

        private IResult<object> Get(string name)
        {
            if (name == null)
                return FailureResult<object>.Instance;
            if (_namedCache.ContainsKey(name))
                return Result.Success(_namedCache[name]);
            if (!_validNames.Contains(name))
                return FailureResult<object>.Instance;
            var env = _environmentFactory.Create(name);
            if (env.HasValue)
            {
                _namedCache.Add(name, env.Value);
                return env;
            }

            return FailureResult<object>.Instance;
        }
    }
}
