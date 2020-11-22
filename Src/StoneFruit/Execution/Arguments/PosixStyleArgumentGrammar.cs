using System;
using System.Linq;
using ParserObjects;
using static ParserObjects.ParserMethods;
using static ParserObjects.ParserMethods<char>;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A grammar for posix-style arguments
    /// </summary>
    public static class PosixStyleArgumentGrammar
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

            var whitespace = Whitespace();

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            // The name can be numbers or symbols or anything besides whitespace.
            var name = Match(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ListCharToString(true);

            var singleDash = Match('-');
            var doubleDash = Match("--");
            var singleIdChar = Match(char.IsLetterOrDigit);

            // '--' <name> '=' <value>
            var longEqualsNamedArg = Rule(
                doubleDash,
                name,
                Match('='),
                value,

                (s, n, e, v) => (IParsedArgument)new ParsedNamedArgument(n, v)
            );

            // '--' <name> <ws> <value>
            var longImpliedNamedArg = Rule(
                doubleDash,
                name,
                whitespace,
                value,

                (s, n, e, v) => (IParsedArgument)new ParsedFlagPositionalOrNamedArgument(n, v)
            );

            // '--' <name>
            var longFlagArg = Rule(
                doubleDash,
                name,

                (s, n) => (IParsedArgument)new ParsedFlagArgument(n)
            );

            // We include this case because some short args with a positional following, are treated like
            // a named arg. Provide all cases, because we don't know the intent.
            // '-' <char> <ws> <value>
            var shortImpliedNamedArg = Rule(
                singleDash,
                singleIdChar.Transform(c => c.ToString()),
                whitespace,
                value,

                (dash, n, ws, v) => (IParsedArgument)new ParsedFlagPositionalOrNamedArgument(n, v)
            );

            // '-' <char>*
            var shortFlagArg = Rule(
                singleDash,
                singleIdChar.List(true),

                (s, n) => (IParsedArgument)new MultiParsedFlagArgument(n.Select(x => x.ToString()))
            );

            var positional = value.Transform(v => (IParsedArgument)new ParsedPositionalArgument(v));

            // <named> | <longFlag> | <shortFlag> | <positional>
            var args = First(
                longEqualsNamedArg,
                longImpliedNamedArg,
                longFlagArg,
                shortImpliedNamedArg,
                shortFlagArg,
                positional
            );

            var whitespaceAndArgs = Rule(
                whitespace.Optional(),
                args,

                (ws, arg) => arg
            );

            return whitespaceAndArgs;
        }
    }
}
