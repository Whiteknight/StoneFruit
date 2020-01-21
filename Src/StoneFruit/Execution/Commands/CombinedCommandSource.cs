using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Commands
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class CombinedCommandSource : ICommandSource
    {
        private readonly List<ICommandSource> _sources;

        public CombinedCommandSource()
        {
            _sources = new List<ICommandSource>();
        }

        public ICommandVerb GetCommandInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            return _sources
                .Select(source => source.GetCommandInstance(completeCommand, dispatcher))
                .FirstOrDefault(commandVerb => commandVerb != null);
        }

        public ICommandVerb GetCommandInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) where TCommand : class, ICommandVerb
        {
            return _sources
                .Select(source => source.GetCommandInstance<TCommand>(completeCommand, dispatcher))
                .FirstOrDefault(commandVerb => commandVerb != null);
        }

        public IReadOnlyDictionary<string, Type> GetAll()
        {
            return _sources.SelectMany(s => s.GetAll())
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(g => g.Key, g => g.First().Value);
        }

        public void Add(ICommandSource source)
        {
            if (source == null)
                return;
            _sources.Add(source);
        }

        public ICommandSource Simplify()
        {
            return _sources.Count == 1 ? _sources[0] : this;
        }

        public Type GetCommandTypeByName(string name)
        {
            return _sources
                .Select(source => source.GetCommandTypeByName(name))
                .FirstOrDefault(type => type != null);
        }
    }
}
