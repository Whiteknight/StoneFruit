using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.CommandSources
{
    public class CombinedCommandSource : ICommandSource
    {
        private readonly Queue<ICommandSource> _sources;
        private ICommandSource _current;

        public CombinedCommandSource()
        {
            _sources = new Queue<ICommandSource>();
        }

        public void AddSource(ICommandSource source)
        {
            _sources.Enqueue(source);
        }

        public void Start()
        {
            _current = null;
        }

        public string GetNextCommand()
        {
            if (_current == null)
            {
                StartNextSource();
                if (_current == null)
                    return null;
            }

            while (true)
            {
                var next = _current.GetNextCommand();
                if (!string.IsNullOrEmpty(next))
                    return next;
                StartNextSource();
                if (_current == null)
                    return null;
            }
        }

        private void StartNextSource()
        {
            _current = _sources.Any() ? _sources.Dequeue() : null;
        }
    }
}