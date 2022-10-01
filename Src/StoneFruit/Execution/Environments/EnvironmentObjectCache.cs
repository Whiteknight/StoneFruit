using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Environments
{
    // Object so we can cache the environment context objects
    public sealed class EnvironmentObjectCache
    {
        private readonly Dictionary<string, Dictionary<Type, object>> _cache;

        public EnvironmentObjectCache()
        {
            _cache = new Dictionary<string, Dictionary<Type, object>>();
        }

        public IResult<T> Get<T>(string environment)
            where T : class
        {
            if (!_cache.ContainsKey(environment))
                return FailureResult<T>.Instance;
            var env = _cache[environment];
            if (!env.ContainsKey(typeof(T)))
                return FailureResult<T>.Instance;
            var value = env[typeof(T)] as T;
            return Result.Success(value);
        }

        public void Set<T>(string environment, T value)
            where T : class
        {
            if (!_cache.ContainsKey(environment))
                _cache.Add(environment, new Dictionary<Type, object>());
            var env = _cache[environment];
            if (!env.ContainsKey(typeof(T)))
                env.Add(typeof(T), value);
            else
                env[typeof(T)] = value;
        }
    }
}
