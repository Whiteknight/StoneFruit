using System;
using System.Collections.Generic;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Scripts.Formatting;

namespace StoneFruit.Execution.Arguments
{
    public class ArgumentParserSetup : IArgumentParserSetup
    {
        private IParser<char, IParsedArgument> _argParser;
        private IParser<char, string> _verbParser;
        private IParser<char, CommandFormat> _scriptParser;

        public CommandParser Build()
        {
            var argParser = _argParser ?? SimplifiedArgumentGrammar.GetParser();
            var verbParser = _verbParser ?? VerbGrammar.GetParser();
            var scriptParser = _scriptParser ?? ScriptFormatGrammar.CreateParser(verbParser);
            return new CommandParser(verbParser, argParser, scriptParser);
        }

        // TODO: Should be able to inject the overall command parser (go from verb-object to object-verb, etc)
        // TODO: CommandParser needs to be abstracted?

        public IArgumentParserSetup UseVerbParser(IParser<char, string> verbParser)
        {
            if (verbParser == null)
            {
                _verbParser = null;
                return this;
            }

            EnsureVerbParserNotSet();
            _verbParser = verbParser;
            return this;
        }

        public IArgumentParserSetup UseArgumentParser(IParser<char, IParsedArgument> argParser)
        {
            if (argParser == null)
            {
                _argParser = null;
                return this;
            }

            EnsureArgumentParserNotSet();
            _argParser = argParser;
            return this;
        }

        public IArgumentParserSetup UseArgumentParser(IParser<char, IEnumerable<IParsedArgument>> argParser)
        {
            var parser = argParser.Flatten<char, IEnumerable<IParsedArgument>, IParsedArgument>();
            return UseArgumentParser(parser);
        }

        public IArgumentParserSetup UseScriptParser(IParser<char, CommandFormat> scriptParser)
        {
            if (scriptParser == null)
            {
                _scriptParser = null;
                return this;
            }

            EnsureScriptParserNotSet();
            _scriptParser = scriptParser;
            return this;
        }

        private void EnsureArgumentParserNotSet()
        {
            if (_argParser != null)
                throw new Exception("Argument parser is already set for this builder. You cannot set a second argument parser.");
        }

        private void EnsureVerbParserNotSet()
        {
            if (_verbParser != null)
                throw new Exception("Verb parser is already set for this builder. You cannot set a second verb parser.");
        }

        private void EnsureScriptParserNotSet()
        {
            if (_scriptParser != null)
                throw new Exception("Script parser is already set for this builder. You cannot set a second script parser.");
        }
    }
}
