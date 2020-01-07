using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Parsers.Specialty;
using ParserMethods = ParserObjects.Parsers.ParserMethods;

namespace StoneFruit.Execution.Arguments
{
    public class PosixStyleArgumentGrammar
    {
        public static IParser<char, IArgument> GetParser()
        {
            var doubleQuotedString = ProgrammingParserMethods.StrippedDoubleQuotedStringWithEscapedQuotes();
            var singleQuotedString = ProgrammingParserMethods.StrippedSingleQuotedStringWithEscapedQuotes();
            var unquotedValue = ParserMethods.Match<char>(c => !char.IsWhiteSpace(c)).List(c => new string(Enumerable.ToArray<char>(c)), true);

            var value = ParserMethods.First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var name = ProgrammingParserMethods.CStyleIdentifier();

            var namedArg = ParserMethods.Rule(
                ParserMethods.Match("--", c => c),
                name,
                ParserMethods.Match("=", c => c),
                value,

                (s, n, e, v) => new NamedArgument(n, v)
            );

            var longFlagArg = ParserMethods.Rule(
                ParserMethods.Match("--", c => c),
                name,

                (s, n) => new FlagArgument(n)
            );

            // TODO: "-abc" same as "-a -b -c"
            var shortFlagArg = ParserMethods.Rule(
                ParserMethods.Match("-", c => c),
                name,

                (s, n) => new FlagArgument(n)
            );

            var args = ParserMethods.First<char, IArgument>(
                namedArg,
                shortFlagArg,
                value.Transform(v => new PositionalArgument(v))
            );

            var whitespace = ParserObjects.Parsers.Specialty.ParserMethods.Whitespace();

            return ParserMethods.Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}