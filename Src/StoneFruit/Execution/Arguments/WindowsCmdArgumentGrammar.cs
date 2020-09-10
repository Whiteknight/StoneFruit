using ParserObjects;
using ParserObjects.Parsers;
using System;
using System.Linq;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.CStyleParserMethods;
using static ParserObjects.Parsers.Specialty.QuotedParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// An argument grammar for windows CMD-style arguments
    /// </summary>
    public static class WindowsCmdArgumentGrammar
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

            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var name = Identifier();

            // '/' <name> ':' <value>
            var namedArg = Rule(
                Match('/'),
                name,
                Match(':'),
                value,

                (s, n, e, v) => new ParsedNamedArgument(n, v)
            );

            // '-' <name> <whitespace> <value>
            var maybeNamedArg = Rule(
                Match('/'),
                name,
                whitespace,
                value,

                (s, name, e, value) => new ParsedFlagPositionalOrNamedArgument(name, value)
            );

            // '/' <name>
            var flagArg = Rule(
                Match('/'),
                name,

                (s, n) => new ParsedFlagArgument(n)
            );

            // <named> | <flag> | <positional>
            var args = First<char, IParsedArgument>(
                namedArg,
                maybeNamedArg,
                flagArg,
                value.Transform(v => new ParsedPositionalArgument(v))
            );

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}