using System.Collections.Generic;
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
        public static IParser<char, IEnumerable<IArgument>> GetParser()
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
                value,

                (s, n, e, v) => new [] { new NamedArgument(n, v) }
            );

            var flagArg = Rule(
                Match('/'),
                name,

                (s, n) => new [] { new FlagArgument(n) }
            );

            var args = First<char, IEnumerable<IArgument>>(
                namedArg,
                flagArg,
                value.Transform(v => new [] { new PositionalArgument(v) })
            );

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}