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
    /// A grammar for Powershell-style arguments
    /// </summary>
    public class PowershellStyleArgumentGrammar
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

            // '-' <name> <whitespace> <value>
            var namedArg = Rule(
                Match('-'),
                names,
                whitespace,
                values,

                (s, name, e, value) => new IArgument[] { new FlagArgument(name), new PositionalArgument(value), new NamedArgument(name, value)  }
            );

            // '-' <name>
            var longFlagArg = Rule(
                Match('-'),
                names,

                (s, name) => new[] { new FlagArgument(name) }
            );

            // <named> | <longFlag> | <positional>
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