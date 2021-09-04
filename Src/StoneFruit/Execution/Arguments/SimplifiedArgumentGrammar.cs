using System;
using System.Linq;
using ParserObjects;
using static ParserObjects.CStyleParserMethods;
using static ParserObjects.ParserMethods;
using static ParserObjects.ParserMethods<char>;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A grammar for simplified arguments.
    /// </summary>
    public static class SimplifiedArgumentGrammar
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

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            ).Named("Value");

            var names = Identifier();

            // '-' <name>
            var flagArg = Rule(
                Match('-'),
                names,

                (_, name) => new ParsedFlagArgument(name)
            ).Named("Flag");

            // <name> '=' <value>
            var namedArg = Rule(
                names,
                Match('='),
                values,

                (name, _, value) => new ParsedNamedArgument(name, value)
            ).Named("Named");

            // <flag> | <named> | <positional>
            var args = First<IParsedArgument>(
                flagArg,
                namedArg,
                values.Transform(v => new ParsedPositionalArgument(v))
            ).Named("Argument");

            return Rule(
                OptionalWhitespace(),
                args,

                (_, arg) => arg
            ).Named("WhitespaceAndArgument");
        }
    }
}
