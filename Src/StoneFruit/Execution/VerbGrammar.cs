using System;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Grammar for parsing a verb from the beginning of a command string.
    /// </summary>
    public static class VerbGrammar
    {
        private static readonly Lazy<IParser<char, string>> _instance = new Lazy<IParser<char, string>>(GetParserInternal);

        public static IParser<char, string> GetParser() => _instance.Value;

        private static IParser<char, string> GetParserInternal()
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
                whitespace,

                (v, ws) => v
            );
        }
    }
}