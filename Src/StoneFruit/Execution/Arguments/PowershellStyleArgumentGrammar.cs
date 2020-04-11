using System;
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
    /// A grammar for Powershell-style arguments
    /// </summary>
    public class PowershellStyleArgumentGrammar
    {
        private static readonly Lazy<IParser<char, IParsedArgument>> _instance = new Lazy<IParser<char, IParsedArgument>>(GetParserInternal);

        public static IParser<char, IParsedArgument> GetParser() => _instance.Value;

        private static IParser<char, IParsedArgument> GetParserInternal()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            var whitespace = OptionalWhitespace();

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var names = Identifier();

            // Powershell convention doesn't really have a clear way to specify that a switch/value is a
            // named arg or just a switch followed by a positional. Return a combined argument which acts
            // like all three and the user can consume it however they want

            // '-' <name> <whitespace> <value>
            var namedArg = Rule(
                Match('-'),
                names,
                whitespace,
                values,

                (s, name, e, value) => new ParsedFlagPositionalOrNamedArgument(name, value)
            );

            // '-' <name>
            var longFlagArg = Rule(
                Match('-'),
                names,

                (s, name) => new ParsedFlagArgument(name)
            );

            // TODO: Some powershell commandlets also seem to support "'--' <name>" for flags and named args

            // <named> | <longFlag> | <positional>
            var args = First<char, IParsedArgument>(
                namedArg,
                longFlagArg,
                values.Transform(v => new ParsedPositionalArgument(v))
            );

            var whitespaceAndArgs = Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );

            return whitespaceAndArgs;
        }
    }
}