using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Scripts.Formatting;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Scripts
{
    /// <summary>
    /// Handler source which maintains a list of scripts and wraps each script in a handler
    /// </summary>
    public class ScriptHandlerSource : IHandlerSource
    {
        private readonly VerbTrie<Script> _scripts;

        public ScriptHandlerSource()
        {
            _scripts = new VerbTrie<Script>();
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var script = _scripts.Get(arguments);
            if (script == null)
                return null;
            return new ScriptHandler(dispatcher.Parser, script, arguments, dispatcher.State);
        }

        public IEnumerable<IVerbInfo> GetAll() => _scripts.GetAll().Select(kvp => kvp.Value);

        public IVerbInfo GetByName(Verb verb) => _scripts.Get(verb);

        public void AddScript(Verb verb, IEnumerable<string> lines, string description = null, string usage = null, string group = null)
        {
            var scriptLines = lines.OrEmptyIfNull().ToList();
            if (scriptLines.Count == 0)
                return;
            var script = new Script(verb, scriptLines, description, usage, group);
            _scripts.Insert(verb, script);
        }

        public void Initialize(IVerbExtractor _)
        {
            // No initialization necessary
        }

        public int Count => _scripts.Count;

        // This is going to stay a private child class because it has ugly mutable state
        // and it's only used here.
        private class Script : IVerbInfo
        {
            private readonly IReadOnlyList<string> _lines;
            private IReadOnlyList<CommandFormat> _formats;

            public Script(Verb verb, IReadOnlyList<string> lines, string description, string usage, string group)
            {
                _lines = lines;
                Verb = verb;
                Description = description;
                Usage = usage;
                Group = group;
            }

            public Verb Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public string Group { get; }
            public bool ShouldShowInHelp => true;

            public IEnumerable<CommandFormat> GetFormats(ICommandParser parser)
            {
                if (_formats != null)
                    return _formats;
                _formats = _lines
                    .Select(l => parser.ParseScript(l))
                    .ToList();
                return _formats;
            }
        }

        // We need to keep ScriptHandler private child class, so it doesn't get scooped up
        // during DI container scan
        private class ScriptHandler : IHandler
        {
            private readonly ICommandParser _parser;
            private readonly Script _script;
            private readonly IArguments _arguments;
            private readonly EngineState _state;

            public ScriptHandler(ICommandParser parser, Script script, IArguments arguments, EngineState state)
            {
                _parser = parser;
                _script = script;
                _arguments = arguments;
                _state = state;
            }

            public void Execute()
            {
                // Get the format objects, parsing them if necessary
                var formats = _script.GetFormats(_parser);
                foreach (var lineFormat in formats)
                {
                    // Fill in arguments to the formats to create the command
                    var formattedArguments = lineFormat.Format(_arguments);
                    _arguments.ResetAllArguments();

                    // Add the command to the EngineState for execution after this
                    _state.Commands.Append(new ArgumentsOrString(formattedArguments));
                }
            }
        }
    }
}
