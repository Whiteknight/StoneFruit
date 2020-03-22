﻿using System.Collections.Generic;
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
        private readonly Dictionary<string, Script> _scripts;

        public ScriptHandlerSource()
        {
            _scripts = new Dictionary<string, Script>();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            if (!_scripts.ContainsKey(command.Verb))
                return null;
            return new ScriptHandler(dispatcher.Parser, _scripts[command.Verb], command, dispatcher.State);
        }

        public IEnumerable<IVerbInfo> GetAll() => _scripts.Values;

        public IVerbInfo GetByName(string name) => _scripts.ContainsKey(name) ? _scripts[name] : null;

        public void AddScript(string verb, IEnumerable<string> lines, string description = null, string usage = null)
        {
            var script = new Script(verb, lines.OrEmptyIfNull().ToList(), description, usage);
            _scripts.Add(verb, script);
        }

        // TODO: Ability to read in a script from a file

        public int Count => _scripts.Count;

        private class Script : IVerbInfo
        {
            private readonly IReadOnlyList<string> _lines;
            private IReadOnlyList<CommandFormat> _formats;

            public Script(string verb, IReadOnlyList<string> lines, string description, string usage)
            {
                _lines = lines;
                Verb = verb;
                Description = description;
                Usage = usage;
            }

            public string Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public bool ShouldShowInHelp => true;

            public IEnumerable<CommandFormat> GetFormats(CommandParser parser)
            {
                if (_formats != null)
                    return _formats;
                var formats = new List<CommandFormat>();
                foreach (var line in _lines)
                {
                    var result = parser.ParseScript(line);
                    formats.Add(result);
                }

                _formats = formats;
                return formats;
            }
        }

        private class ScriptHandler : IHandler
        {
            private readonly CommandParser _parser;
            private readonly Script _script;
            private readonly Command _command;
            private readonly EngineState _state;

            public ScriptHandler(CommandParser parser, Script script, Command command, EngineState state)
            {
                _parser = parser;
                _script = script;
                _command = command;
                _state = state;
            }

            public void Execute()
            {
                // Get the format objects, parsing them if necessary
                var formats = _script.GetFormats(_parser);
                foreach (var lineFormat in formats)
                {
                    // Fill in arguments to the formats to create the command
                    var command = lineFormat.Format(_command.Arguments);
                    _command.Arguments.ResetAllArguments();

                    // Add the command to the EngineState for execution after this
                    _state.AddCommand(command);
                }
            }
        }
    }
}
