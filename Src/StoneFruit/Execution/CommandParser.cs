using ParserObjects;
using ParserObjects.Sequences;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    public class CommandParser
    {
        private readonly IParser<char, CommandArguments> _argsParser;
        private readonly IParser<char, CompleteCommand> _commandParser;

        public CommandParser(IParser<char, CommandArguments> argParser = null)
        {
            _argsParser = argParser ?? CommandArgumentsGrammar.GetParser();
            _commandParser = CompleteCommandGrammar.GetParser(_argsParser);
        }

        public static CommandParser GetDefault() => new CommandParser();

        public CompleteCommand ParseCommand(string line)
        {
            var sequence = new StringCharacterSequence(line);
            return _commandParser.Parse(sequence).Value;
        }

        public CommandArguments ParseArguments(string args)
        {
            var sequence = new StringCharacterSequence(args);
            return _argsParser.Parse(sequence).Value;
        }
    }
}