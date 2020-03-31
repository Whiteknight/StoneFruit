using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.CommandSources
{
    /// <summary>
    /// Ordered collection of ICommandSources. A source is drained and then removed from
    /// the list.
    /// </summary>
    public class CommandSourceCollection 
    {
        private readonly LinkedList<ICommandSource> _sources;

        public CommandSourceCollection()
        {
            _sources = new LinkedList<ICommandSource>();
        }

        public void AddToEnd(ICommandSource source)
        {
            if (source == null)
                return;
            _sources.AddLast(source);
        }

        public void AddToEnd(params string[] commands) => AddToEnd(new QueueCommandSource(commands));

        public void AddToEnd(EventScript script, CommandParser parser)
            => AddToEnd(new ScriptCommandSource(script, parser, new IArgument[0]));

        public void AddToEnd(EventScript script, CommandParser parser, params IArgument[] args)
            => AddToEnd(new ScriptCommandSource(script, parser, args));

        public void AddToEnd(EventScript script, CommandParser parser, params (string, string)[] args)
        {
            var argsList = args.Select(t => new NamedArgumentAccessor(t.Item1, t.Item2)).Cast<IArgument>().ToArray();
            AddToEnd(new ScriptCommandSource(script, parser, argsList));
        }

        public void AddToBeginning(ICommandSource source)
        {
            if (source == null)
                return;
            _sources.AddFirst(source);
        }

        public CommandObjectOrString GetNextCommand()
        {
            while (true)
            {
                if (_sources.Count == 0)
                    return null;

                var firstSource = _sources.First.Value;

                var next = firstSource.GetNextCommand();
                if (next != null)
                    return next;

                _sources.RemoveFirst();
            }
        }
    }
}