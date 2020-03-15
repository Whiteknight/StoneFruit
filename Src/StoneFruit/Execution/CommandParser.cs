using System;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    public class CommandParser
    {
        private readonly IParser<char, IArgument> _argsParser;
        private readonly IParser<char, string> _verbParser;

        public CommandParser(IParser<char, IArgument> argParser = null)
        {
            // TODO: VerbParser should be pluggable
            _argsParser = argParser ?? SimplifiedArgumentGrammar.GetParser();
            _verbParser = VerbGrammar.GetParser();
        }

        public static CommandParser GetDefault() => new CommandParser();

        public static Command ParseCommand(IParser<char, string> verbs, IParser<char, IArgument> args, string line)
        {
            var sequence = new StringCharacterSequence(line);
            var verb = verbs.Parse(sequence).Value;
            var rawArgs = sequence.GetRemainder();
            var argsList = args.List().Parse(sequence).Value.ToList();
            if (!sequence.IsAtEnd)
            {
                var remainder = sequence.GetRemainder();
                throw new Exception($"Could not parse all arguments. '{remainder}' fails at {sequence.CurrentLocation}");
            }

            var cmdArgs = new CommandArguments(rawArgs, argsList);
            return new Command(verb, cmdArgs, line);
        }

        public Command ParseCommand(string line) => ParseCommand(_verbParser, _argsParser, line);

        public CommandArguments ParseArguments(string args) => _argsParser.ParseArguments(args);

        // TODO: Helper methods here to parse all the different dialects, in case the default is not wanted
    }
}