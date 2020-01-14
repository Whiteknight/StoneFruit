using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.ProgrammingParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParsersMethods;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument grammar for windows CMD-style arguments
    /// </summary>
    public class WindowsCmdArgumentGrammar
    {
        public static IParser<char, IArgument> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedStringWithEscapedQuotes();
            var singleQuotedString = StrippedSingleQuotedStringWithEscapedQuotes();
            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));
            var whitespace = Whitespace();

            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var name = CStyleIdentifier();

            var namedArg = Rule(
                Match('/'),
                name,
                Match(':'),
                // TODO: don't backtrack here
                value,

                (s, n, e, v) => new NamedArgument(n, v)
            );

            var flagArg = Rule(
                Match('/'),
                name,

                (s, n) => new FlagArgument(n)
            );

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