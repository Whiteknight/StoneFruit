using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments
{
    /// <summary>
    /// An IEnvironmentCollection implementation which uses an IEnvironmentFactory to create environments
    /// on demand.
    /// </summary>
    public class FactoryEnvironmentCollection : IEnvironmentCollection
    {
        private readonly IReadOnlyList<string> _nameList;
        private readonly HashSet<string> _validNames;

        private IResult<object> _current;
        private IResult<string> _currentName;

        public FactoryEnvironmentCollection(EnvironmentsList environmentList)
        {
            var environments = environmentList.ValidNames;
            if (environments == null || environments.Count == 0)
                environments = new[] { Constants.EnvironmentNameDefault };
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
            if (!_validNames.Contains(name))
                throw new InvalidOperationException($"Environment {name} does not exist");
            _currentName = Result.Success(name);
        }
    }
}
