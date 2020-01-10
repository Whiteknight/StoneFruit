using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.Specialty.ProgrammingParserMethods;
using static ParserObjects.Parsers.ParserMethods;

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
            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c)).List(c => new string(c.ToArray()), true);

            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var name = CStyleIdentifier();

            var namedArg = Rule(
                Match("--", c => c),
                name,
                Match("=", c => c),
                value,

                (s, n, e, v) => new [] { new NamedArgument(n, v) }
            );

            var longFlagArg = Rule(
                Match("--", c => c),
                name,

                (s, n) => new [] { new FlagArgument(n) }
            );

            var shortFlagArg = Rule(
                Match("-", c => c),
                Match<char>(char.IsLetterOrDigit).List(c => c, true),

                (s, n) => n.Select(x =>  new FlagArgument(x.ToString()))
            );

            var args = First<char, IEnumerable<IArgument>>(
                namedArg,
                shortFlagArg,
                value.Transform(v => new [] { new PositionalArgument(v) })
            );

            var whitespace = ParserObjects.Parsers.Specialty.ParserMethods.Whitespace();

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}