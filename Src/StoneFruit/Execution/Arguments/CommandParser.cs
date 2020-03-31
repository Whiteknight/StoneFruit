using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;
using StoneFruit.Execution.Scripts.Formatting;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Arguments
{
    public class CommandParser
    {
        private readonly IParser<char, string> _verbParser;
        private readonly IParser<char, IParsedArgument> _argsParser;
        private readonly IParser<char, CommandFormat> _scriptParser;

        public CommandParser(IParser<char, string> verbParser, IParser<char, IParsedArgument> argParser, IParser<char, CommandFormat> scriptParser)
        {
            Assert.ArgumentNotNull(verbParser, nameof(verbParser));
            Assert.ArgumentNotNull(argParser, nameof(argParser));
            Assert.ArgumentNotNull(scriptParser, nameof(scriptParser));

            _verbParser = verbParser;
            _argsParser = argParser;
            _scriptParser = scriptParser;
        }

        public static CommandParser GetDefault()
        {
            var verbParser = VerbGrammar.GetParser();
            var argParser = SimplifiedArgumentGrammar.GetParser();
            var scriptParser = ScriptFormatGrammar.CreateParser(verbParser);
            return new CommandParser(verbParser, argParser, scriptParser);
        }

        public static Command ParseCommand(IParser<char, string> verbs, IParser<char, IParsedArgument> args, string line)
        {
            var sequence = new StringCharacterSequence(line);
            var verb = verbs.Parse(sequence).Value;
            var rawArgs = sequence.GetRemainder();
            var argsList = args.List().Parse(sequence).Value.ToList();
            if (!sequence.IsAtEnd)
            {
                var remainder = sequence.GetRemainder();
                throw new ParseException($"Could not parse all arguments. '{remainder}' fails at {sequence.CurrentLocation}");
            }

            var cmdArgs = new CommandArguments(rawArgs, argsList);
            return new Command(verb, cmdArgs, raw: line);
        }

        public Command ParseCommand(string line) => ParseCommand(_verbParser, _argsParser, line);

        public CommandArguments ParseArguments(string args) => _argsParser.ParseArguments(args);

        public CommandFormat ParseScript(string script)
        {
            var input = new StringCharacterSequence(script);
            var parseResult = _scriptParser.Parse(input);
            if (!parseResult.Success)
                throw new ParseException($"Could not parse command format string: '{script}'");
            if (!input.IsAtEnd)
                throw new ParseException($"Parse did not complete for format string '{script}'. Unparsed remainder: '{input.GetRemainder()}'");
            return parseResult.Value;
        }
    }
}