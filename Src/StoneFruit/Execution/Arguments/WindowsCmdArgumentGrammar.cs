using System;
using System.Linq;
using ParserObjects;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers.C;
using static ParserObjects.Parsers<char>;

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

            var unquotedValue = Match(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            var whitespace = OptionalWhitespace();

            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            ).Named("Value");

            var name = Identifier();

            // '/' <name> ':' <value>
            var namedArg = Rule(
                Match('/'),
                name,
                Match(':'),
                value,

                (_, n, _, v) => new ParsedNamedArgument(n, v)
            ).Named("Named");

            // '-' <name> <whitespace> <value>
            var maybeNamedArg = Rule(
                Match('/'),
                name,
                whitespace,
                value,

                (_, name, _, value) => new ParsedFlagPositionalOrNamedArgument(name, value)
            ).Named("NamedMaybeValue");

            // '/' <name>
            var flagArg = Rule(
                Match('/'),
                name,

                (_, n) => new ParsedFlagArgument(n)
            ).Named("Flag");

            // <named> | <flag> | <positional>
            var args = First<IParsedArgument>(
                namedArg,
                maybeNamedArg,
                flagArg,
                value.Transform(v => new ParsedPositionalArgument(v))
            ).Named("Argument");

            return Rule(
                whitespace,
                args,

                (_, arg) => arg
            );
        }
    }
}
