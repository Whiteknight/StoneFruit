using System;
using System.Collections.Generic;
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
    /// A grammar for posix-style arguments
    /// </summary>
    public class PosixStyleArgumentGrammar
    {
        private static readonly Lazy<IParser<char, IArgument>> _instance = new Lazy<IParser<char, IArgument>>(GetParserInternal);

        public static IParser<char, IArgument> GetParser() => _instance.Value;

        private static IParser<char, IArgument> GetParserInternal()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c))
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
            var name = Match<char>(c => !char.IsWhiteSpace(c) && c != '=').ListCharToString(true);

            var singleDash = Match('-');
            var doubleDash = Match<char>("--");
            var singleIdChar = Match<char>(char.IsLetterOrDigit);

            // '--' <name> '=' <value>
            var longEqualsNamedArg = Rule(
                doubleDash,
                name,
                Match('='),
                value,

                (s, n, e, v) => new [] { new NamedArgument(n, v) }
            );

            // '--' <name> <ws> <value>
            var longImpliedNamedArg = Rule(
                doubleDash,
                name,
                whitespace,
                value,

                (s, n, e, v) => new IArgument[] { new FlagArgument(n), new NamedArgument(n, v), new PositionalArgument(v) }
            );

            // '--' <name>
            var longFlagArg = Rule(
                doubleDash,
                name,

                (s, n) => new [] { new FlagArgument(n) }
            );

            // We include this case because some short args with a positional following, are treated like
            // a named arg. Provide all cases, because we don't know the intent.
            // '-' <char> <ws> <value>
            var shortImpliedNamedArg = Rule(
                singleDash,
                singleIdChar.Transform(c => c.ToString()),
                whitespace,
                value,

                (dash, n, ws, v) => new IArgument[] { new FlagArgument(n), new NamedArgument(n, v), new PositionalArgument(v) }
            );

            // '-' <char>*
            var shortFlagArg = Rule(
                singleDash,
                singleIdChar.List(true),

                (s, n) => n.Select(x =>  new FlagArgument(x.ToString()))
            );

            // <named> | <longFlag> | <shortFlag> | <positional>
            var args = First<char, IEnumerable<IArgument>>(
                longEqualsNamedArg,
                longImpliedNamedArg,
                longFlagArg,
                shortImpliedNamedArg,
                shortFlagArg,
                value.Transform(v => new [] { new PositionalArgument(v) })
            );

            var whitespaceAndArgs = Rule(
                whitespace.Optional(),
                args,

                (ws, arg) => arg
            );

            return whitespaceAndArgs.Flatten<char, IEnumerable<IArgument>, IArgument>();
        }
    }
}