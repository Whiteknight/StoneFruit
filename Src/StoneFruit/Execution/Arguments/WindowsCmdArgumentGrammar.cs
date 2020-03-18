using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.QuotedParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;
using static ParserObjects.Parsers.Specialty.CStyleParserMethods;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument grammar for windows CMD-style arguments
    /// </summary>
    public class WindowsCmdArgumentGrammar
    {
        // TODO: Unit tests
        public static IParser<char, IArgument> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            var whitespace = OptionalWhitespace();

            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var name = Identifier();

            // '/' <name> ':' <value>
            var namedArg = Rule(
                Match('/'),
                name,
                Match(':'),
                // TODO: don't backtrack here
                value,

                (s, n, e, v) => new NamedArgument(n, v)
            );

            // '/' <name>
            var flagArg = Rule(
                Match('/'),
                name,

                (s, n) => new FlagArgument(n)
            );

            // <named> | <flag> | <positional>
            var args = First<char, IArgument>(
                namedArg,
                flagArg,
                value.Transform(v => new PositionalArgument(v))
            );

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}