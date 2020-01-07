using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Commands
{
    public class CombinedCommandSource : ICommandSource
    {
        private readonly List<ICommandSource> _sources;

        public CombinedCommandSource()
        {
            _sources = new List<ICommandSource>();
        }

        public ICommandVerb GetCommandInstance(CompleteCommand command, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            foreach (var source in _sources)
            {
                var commandVerb = source.GetCommandInstance(command, environments, state, output);
                if (commandVerb != null)
                    return commandVerb;
            }

            return null;
        }

        public IEnumerable<CommandDescription> GetAll()
        {
            // TODO: De-dupe by name. Names will be hidden by earlier sources
            return _sources.SelectMany(s => s.GetAll());
        }

        public void Add(ICommandSource source)
        {
            if (source == null)
                return;
            _sources.Add(source);
        }

        public ICommandSource Simplify()
        {
            if (_sources.Count == 1)
                return _sources[0];
            return this;
        }
    }
}
