using System.Collections.Generic;
using ParserObjects;
using ParserObjects.Sequences;
using StoneFruit.Execution.Scripts;
using StoneFruit.Execution.Scripts.Formatting;
using StoneFruit.Utility;
using static ParserObjects.ParserMethods;
using static ParserObjects.ParserMethods<char>;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// Default ICommandParser implementation which controls parsing commands, arguments
    /// and scripts
    /// </summary>
    public class CommandParser : ICommandParser
    {
        private readonly IParser<char, IReadOnlyList<IParsedArgument>> _argsParser;
        private readonly IParser<char, CommandFormat> _scriptParser;

        public CommandParser(IParser<char, IParsedArgument> argParser, IParser<char, CommandFormat> scriptParser)
        {
            Assert.ArgumentNotNull(argParser, nameof(argParser));
            Assert.ArgumentNotNull(scriptParser, nameof(scriptParser));

            _argsParser = Rule(
                argParser.List(),
                OptionalWhitespace(),
                (args, _) => args
            );
            _scriptParser = scriptParser;
        }

        /// <summary>
        /// Get the default CommandParser instance with default parser objects configured
        /// </summary>
        /// <returns></returns>
        public static CommandParser GetDefault()
        {
            var argParser = SimplifiedArgumentGrammar.GetParser();
            var scriptParser = ScriptFormatGrammar.GetParser();
            return new CommandParser(argParser, scriptParser);
        }

        /// <summary>
        /// Parse the given line of text as an IArguments
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public IArguments ParseCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return SyntheticArguments.Empty();

            var sequence = new StringCharacterSequence(command);
            var argsList = _argsParser.Parse(sequence).Value;
            if (sequence.IsAtEnd)
                return new ParsedArguments(argsList, command);

            var remainder = sequence.GetRemainder();
            throw new ParseException($"Could not parse all arguments. '{remainder}' fails at {sequence.CurrentLocation}");
        }

        /// <summary>
        /// Parse the given line of script as a CommandFormat to be used for creating commands
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
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
