using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Parsers.Specialty;
using ParserMethods = ParserObjects.Parsers.ParserMethods;

namespace StoneFruit.Execution.Arguments
{
    public class WindowsCmdArgumentGrammar
    {
        public static IParser<char, IEnumerable<IArgument>> GetParser()
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
                ParserMethods.Match("/", c => c),
                name,
                ParserMethods.Match(":", c => c),
                value,

                (s, n, e, v) => new [] { new NamedArgument(n, v) }
            );

            var flagArg = ParserMethods.Rule(
                ParserMethods.Match("/", c => c),
                name,

                (s, n) => new [] { new FlagArgument(n) }
            );

            var args = ParserMethods.First<char, IEnumerable<IArgument>>(
                namedArg,
                flagArg,
                value.Transform(v => new [] { new PositionalArgument(v) })
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