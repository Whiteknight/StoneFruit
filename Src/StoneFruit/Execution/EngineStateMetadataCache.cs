using System.Collections;
using System.Collections.Generic;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Metadata storage for the EngineState
    /// </summary>
    public class EngineStateMetadataCache : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _metadata;

        public EngineStateMetadataCache()
        {
            _metadata = new Dictionary<string, object>();
        }

        public void Add(string name, object value, bool allowOverwrite = true)
        {
            if (!_metadata.ContainsKey(name))
            {
                _metadata.Add(name, value);
                return;
            }

            if (!allowOverwrite)
                return;

            _metadata.Remove(name);
            _metadata.Add(name, value);
        }

        public IResult<object> Get(string name)
            => _metadata.ContainsKey(name) ? new SuccessResult<object>(_metadata[name]) : FailureResult<object>.Instance;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _metadata.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _metadata.GetEnumerator();

        public void Remove(string name)
        {
            if (_metadata.ContainsKey(name))
                _metadata.Remove(name);
        }
    }
}
