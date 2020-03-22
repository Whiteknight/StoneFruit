using System.Collections.Generic;

namespace StoneFruit.Execution.CommandSources
{
    /// <summary>
    /// Ordered collection of ICommandSources
    /// </summary>
    public class CommandSourceCollection 
    {
        private readonly LinkedList<Node> _sources;

        public CommandSourceCollection()
        {
            _sources = new LinkedList<Node>();
        }

        public void AddToEnd(ICommandSource source)
        {
            _sources.AddLast(new Node(source));
        }

        public void AddToBeginning(ICommandSource source)
        {
            _sources.AddFirst(new Node(source));
        }

        public CommandObjectOrString GetNextCommand()
        {
            while (true)
            {
                if (_sources.Count == 0)
                    return null;

                var firstSource = _sources.First.Value;
                firstSource.Start();

                var next = firstSource.Source.GetNextCommand();
                if (next != null)
                    return next;

                _sources.RemoveFirst();
            }
        }

        private class Node
        {
            private bool _isStarted;
            public Node(ICommandSource source)
            {
                Source = source;
                _isStarted = false;
            }

            public ICommandSource Source { get; }

            public void Start()
            {
                if (_isStarted)
                    return;
                Source.Start();
                _isStarted = true;
            }
        }
    }
}