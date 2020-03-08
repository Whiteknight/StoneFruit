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
    /// A grammar for simplified arguments
    /// </summary>
    public class SimplifiedArgumentGrammar
    {
        public static IParser<char, IArgument> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var names = Identifier();

            // TODO: Figure out what we want this syntax to be, it should be as simple and ceremony-free as possible
            // '-' <name>
            var flagArg = Rule(
                Match('-'),
                names,

                (start, name) => new FlagArgument(name)
            );

            // <name> '=' <value>
            var namedArg = Rule(
                names,
                Match('='),
                values,

                (name, equals, value) => new NamedArgument(name, value) 
            );

            // <flag> | <named> | <positional>
            var args = First<char, IArgument>(
                flagArg,
                namedArg,
                values.Transform(v => new PositionalArgument(v) )
            );

            var whitespace = Whitespace();

            return Rule(
                whitespace.Optional(),
                args,

                (ws, arg) => arg
            );
        }
    }
}