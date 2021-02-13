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

        public IResult<object> Create(string name)
        {
            if (_environments.ContainsKey(name))
                return Result.Success(_environments[name]);
            return FailureResult<object>.Instance;
        }

        public IReadOnlyCollection<string> ValidEnvironments { get; }
    }
}
