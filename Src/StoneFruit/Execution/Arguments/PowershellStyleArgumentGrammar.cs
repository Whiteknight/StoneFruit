using System;
using System.Linq;
using ParserObjects;
using static ParserObjects.CStyleParserMethods;
using static ParserObjects.ParserMethods;
using static ParserObjects.ParserMethods<char>;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A grammar for Powershell-style arguments
    /// </summary>
    public static class PowershellStyleArgumentGrammar
    {
        private static readonly Lazy<IParser<char, IParsedArgument>> _instance = new Lazy<IParser<char, IParsedArgument>>(GetParserInternal);

        public static IParser<char, IParsedArgument> GetParser() => _instance.Value;

        private static IParser<char, IParsedArgument> GetParserInternal()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var unquotedValue = Match(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            var whitespace = OptionalWhitespace();

            // '--' | '-'
            var nameStart = First(
                Match("--").Transform(c => '-'),
                Match('-')
            );

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

            // <nameStart> <name> <whitespace> <value>
            var namedArg = Rule(
                nameStart,
                names,
                whitespace,
                values,

                (s, name, e, value) => new ParsedFlagPositionalOrNamedArgument(name, value)
            );

            // <nameStart> <name>
            var longFlagArg = Rule(
                nameStart,
                names,

                (s, name) => new ParsedFlagArgument(name)
            );

            // <named> | <longFlag> | <positional>
            var args = First<IParsedArgument>(
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
