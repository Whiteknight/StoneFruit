using System.Collections.Generic;
using ParserObjects;
using ParserObjects.Sequences;
using StoneFruit.Execution.Scripts.Formatting;

namespace StoneFruit.Execution.Scripts
{
    /// <summary>
    /// Handler source which maintains a list of scripts and wraps each script in a handler
    /// </summary>
    public class ScriptHandlerSource : IHandlerSource
    {
        private readonly Dictionary<string, Script> _scripts;
        private readonly IParser<char, CommandFormat> _formatParser;

        public ScriptHandlerSource()
        {
            _scripts = new Dictionary<string, Script>();
            _formatParser = ScriptFormatGrammar.CreateParser();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            if (!_scripts.ContainsKey(command.Verb))
                return null;
            return new ScriptHandler(_scripts[command.Verb], command, dispatcher.State);
        }

        public IHandlerBase GetInstance<TCommand>(Command command, CommandDispatcher dispatcher) 
            where TCommand : class, IHandlerBase 
            => null;

        public IEnumerable<IVerbInfo> GetAll() => _scripts.Values;

        public IVerbInfo GetByName(string name) => _scripts.ContainsKey(name) ? _scripts[name] : null;

        public void AddScript(string verb, IEnumerable<string> lines, string description = null, string usage = null)
        {
            var formats = new List<CommandFormat>();
            foreach (var line in lines)
            {
                var input = new StringCharacterSequence(line);
                var parseResult = _formatParser.Parse(input);
                if (!parseResult.Success)
                    throw new ScriptParseException($"Could not parse command format string: '{line}'");
                if (!input.IsAtEnd)
                    throw new ScriptParseException($"Parse did not complete for format string '{line}'. Unparsed remainder: '{input.GetRemainder()}'");
                formats.Add(parseResult.Value);
            }

            var script = new Script(verb, formats, description, usage);
            _scripts.Add(verb, script);
        }

        // TODO: Ability to read in a script from a file

        public int Count => _scripts.Count;

        private class Script : IVerbInfo
        {
            public IReadOnlyList<CommandFormat> Lines { get; }

            public Script(string verb, IReadOnlyList<CommandFormat> lines, string description, string usage)
            {
                Lines = lines;
                Verb = verb;
                Description = description;
                Usage = usage;
            }

            public string Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public bool ShouldShowInHelp => true;
        }

        private class ScriptHandler : IHandler
        {
            private readonly Script _script;
            private readonly Command _command;
            private readonly EngineState _state;

            public ScriptHandler(Script script, Command command, EngineState state)
            {
                _script = script;
                _command = command;
                _state = state;
            }

            public void Execute()
            {
                foreach (var lineFormat in _script.Lines)
                {
                    var command = lineFormat.Format(_command.Arguments);
                    _command.Arguments.ResetAllArguments();
                    _state.AddCommand(command);
                }
            }
        }
    }
}
