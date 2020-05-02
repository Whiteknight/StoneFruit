using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Scripts.Formatting;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Sets up the parsers
    /// </summary>
    public class ParserSetup : IParserSetup
    {
        private IParser<char, IParsedArgument> _argParser;
        private IParser<char, string> _verbParser;
        private IParser<char, CommandFormat> _scriptParser;

        private ICommandParser _parser;

        public void BuildUp(IServiceCollection services)
        {
            var parser = Build();
            services.AddSingleton(parser);
        }

        public ICommandParser Build()
        {
            if (_parser != null)
                return _parser;

            var argParser = _argParser ?? SimplifiedArgumentGrammar.GetParser();
            var verbParser = _verbParser ?? VerbGrammar.GetParser();
            var scriptParser = _scriptParser ?? ScriptFormatGrammar.CreateParser(verbParser);
            return new CommandParser(verbParser, argParser, scriptParser);
        }

        public IParserSetup UseParser(ICommandParser parser)
        {
            if (parser == null)
            {
                _parser = null;
                return this;
            }

            EnsureCanSetCommandParser();
            _parser = parser;
            return this;
        }

        public IParserSetup UseVerbParser(IParser<char, string> verbParser)
        {
            if (verbParser == null)
            {
                _verbParser = null;
                return this;
            }

            EnsureCanSetVerbParser();
            _verbParser = verbParser;
            return this;
        }

        public IParserSetup UseArgumentParser(IParser<char, IParsedArgument> argParser)
        {
            if (argParser == null)
            {
                _argParser = null;
                return this;
            }

            EnsureCanSetArgumentParser();
            _argParser = argParser;
            return this;
        }

        public IParserSetup UseArgumentParser(IParser<char, IEnumerable<IParsedArgument>> argParser)
        {
            var parser = argParser.Flatten<char, IEnumerable<IParsedArgument>, IParsedArgument>();
            return UseArgumentParser(parser);
        }

        public IParserSetup UseScriptParser(IParser<char, CommandFormat> scriptParser)
        {
            if (scriptParser == null)
            {
                _scriptParser = null;
                return this;
            }

            EnsureCanSetScriptParser();
            _scriptParser = scriptParser;
            return this;
        }

        private void EnsureCanSetArgumentParser()
        {
            if (_argParser != null)
                throw new EngineBuildException("Argument parser is already set for this builder. You cannot set a second argument parser.");
            if (_parser != null)
                throw new EngineBuildException("Command parser is already set for this builder. You cannot set an Argument parser and a Command parser at the same time.");
        }

        private void EnsureCanSetVerbParser()
        {
            if (_verbParser != null)
                throw new EngineBuildException("Verb parser is already set for this builder. You cannot set a second verb parser.");
            if (_parser != null)
                throw new EngineBuildException("Command parser is already set for this builder. You cannot set a Verb parser and a Command parser at the same time.");
        }

        private void EnsureCanSetScriptParser()
        {
            if (_scriptParser != null)
                throw new EngineBuildException("Script parser is already set for this builder. You cannot set a second script parser.");
            if (_parser != null)
                throw new EngineBuildException("Command parser is already set for this builder. You cannot set a Script parser and a Command parser at the same time.");
        }


        private void EnsureCanSetCommandParser()
        {
            if (_parser != null)
                throw new EngineBuildException("Command parser is already set. You cannot set a second command parser.");
            if (_argParser != null || _verbParser != null || _scriptParser != null)
                throw new EngineBuildException("Cannot set Command parser if you have already set one of Verb/Argument/Script parsers");
        }
    }
}
