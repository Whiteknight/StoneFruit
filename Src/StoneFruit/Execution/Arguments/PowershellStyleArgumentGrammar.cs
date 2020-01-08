using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.ProgrammingParserMethods;

namespace StoneFruit.Execution.Arguments
{
    public class PowershellStyleArgumentGrammar
    {
        public static IParser<char, IEnumerable<IArgument>> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedStringWithEscapedQuotes();
            var singleQuotedString = StrippedSingleQuotedStringWithEscapedQuotes();
            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c)).List(c => new string(c.ToArray()), true);
            var whitespace = ParserObjects.Parsers.Specialty.ParserMethods.Whitespace();

            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var names = CStyleIdentifier();

            // Powershell convention doesn't really have a clear way to specify that a switch/value is a
            // named arg or just a switch followed by a positional. We'll return all three versions so
            // downstream we can access it however it makes sense (but if you .Consume() one, it breaks your
            // ability to access things a different way)
            var namedArg = Rule(
                Match("-", c => c),
                names,
                whitespace,
                values,

                (s, name, e, value) => new IArgument[] { new FlagArgument(name), new PositionalArgument(value), new NamedArgument(name, value)  }
            );

            var longFlagArg = Rule(
                Match("-", c => c),
                names,

                (s, name) => new[] { new FlagArgument(name) }
            );

            var args = First<char, IEnumerable<IArgument>>(
                namedArg,
                longFlagArg,
                values.Transform(v => new[] { new PositionalArgument(v) })
            );

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}