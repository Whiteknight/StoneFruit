using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.ProgrammingParserMethods;
using static ParserObjects.Parsers.Specialty.ParserMethods;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A grammar for simplified arguments
    /// </summary>
    public class SimplifiedArgumentGrammar
    {
        public static IParser<char, IEnumerable<IArgument>> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedStringWithEscapedQuotes();
            var singleQuotedString = StrippedSingleQuotedStringWithEscapedQuotes();
            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c)).List(c => new string(c.ToArray()), true);

            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var names = CStyleIdentifier();

            // TODO: Figure out what we want this syntax to be, we want it as simple and ceremony-free as possible
            var flagArg = Rule(
                Match("-", c => c),
                names,

                (start, name) => new FlagArgument(name)
            );

            var namedArg = Rule(
                names,
                Match("=", c => c),
                values,

                (name, equals, value) => new NamedArgument(name, value) 
            );

            var args = First<char, IArgument>(
                flagArg,
                namedArg,
                values.Transform(v => new PositionalArgument(v) )
            );

            var whitespace = Whitespace();

            return Rule(
                whitespace,
                args,

                (ws, arg) => new [] { arg }
            );
        }
    }
}