using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution.Arguments;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Grammar for parsing a complete command with verb and arguments
    /// </summary>
    public static class CompleteCommandGrammar
    {
        public static IParser<char, CompleteCommand> GetParser(IParser<char, CommandArguments> argParser)
        {
            var firstChar = Match<char>(c => c == '_' || char.IsLetter(c));
            var bodyChars = Match<char>(c => c == '_' || c == '-' || char.IsLetterOrDigit(c));
            var verb = Rule(
                firstChar,
                bodyChars.ListCharToString(),
                (first, rest) => (first + rest).ToLowerInvariant()
            );
            var whitespace = Whitespace().Optional();

            return Rule(
                verb,
                argParser,
                whitespace,
                End<char>(),

                (v, args, ws, end) => new CompleteCommand(v, args)
            );
        }
    }
}