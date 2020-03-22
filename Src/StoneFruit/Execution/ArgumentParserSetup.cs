using System;
using System.Collections.Generic;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    public class ArgumentParserSetup : IArgumentParserSetup
    {
        private IParser<char, IArgument> _argParser;

        public CommandParser Build()
        {
            return _argParser == null ? CommandParser.GetDefault() : new CommandParser(_argParser);
        }

        // TODO: Should be able to inject the verb parser also
        // TODO: Should be able to inject the overall command parser (go from verb-object to object-verb, etc)

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public IArgumentParserSetup UseArgumentParser(IParser<char, IArgument> argParser)
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

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public IArgumentParserSetup UseArgumentParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            var parser = argParser.Flatten<char, IEnumerable<IArgument>, IArgument>();
            return UseArgumentParser(parser);
        }

        private void EnsureArgumentParserNotSet()
        {
            if (_argParser != null)
                throw new Exception("Argument parser is already set for this builder. You cannot set a second argument parser.");
        }
    }
}
