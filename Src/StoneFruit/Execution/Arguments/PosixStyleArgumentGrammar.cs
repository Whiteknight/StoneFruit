using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.ProgrammingParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A grammar for posix-style arguments
    /// </summary>
    public class PosixStyleArgumentGrammar
    {
        public static IParser<char, IEnumerable<IArgument>> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedStringWithEscapedQuotes();

            var singleQuotedString = StrippedSingleQuotedStringWithEscapedQuotes();

            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            var whitespace = Whitespace();

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            // TODO: we can be more flexible here, because the "--" prefix demarcates the name unambiguously
            var name = CStyleIdentifier();

            // '--' <name> '=' <value>
            var namedArg = Rule(
                Match<char>("--"),
                name,
                Match('='),
                // TODO: Don't backtrack here.
                value,

                (s, n, e, v) => new [] { new NamedArgument(n, v) }
            );

            // '--' <name>
            var longFlagArg = Rule(
                Match<char>("--"),
                name,

                (s, n) => new [] { new FlagArgument(n) }
            );

            // '-' <char>*
            var shortFlagArg = Rule(
                Match('-'),
                Match<char>(char.IsLetterOrDigit).List(true),

                (s, n) => n.Select(x =>  new FlagArgument(x.ToString()))
            );

            // <named> | <longFlag> | <shortFlag> | <positional>
            var args = First<char, IEnumerable<IArgument>>(
                namedArg,
                longFlagArg,
                shortFlagArg,
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