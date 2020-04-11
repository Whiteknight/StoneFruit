using System.Collections.Generic;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Metadata storage for the EngineState
    /// </summary>
    public class EngineStateMetadataCache
    {
        private readonly Dictionary<string, object> _metadata;

        public EngineStateMetadataCache()
        {
            _metadata = new Dictionary<string, object>();
        }

        public void Add(string name, object value, bool allowOverwrite = true)
        {
            if (_metadata.ContainsKey(name))
            {
                if (!allowOverwrite)
                    return;
                _metadata.Remove(name);
            }

            _metadata.Add(name, value);
        }

        public object Get(string name)
        {
            if (!_metadata.ContainsKey(name))
                return null;
            return _metadata[name];
        }

        public void Remove(string name)
        {
            if (_metadata.ContainsKey(name))
                _metadata.Remove(name);
        }
    }
}