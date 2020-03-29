﻿using System;
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
    /// An argument grammar for windows CMD-style arguments
    /// </summary>
    public class WindowsCmdArgumentGrammar
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

                (s, n, e, v) => new NamedArgument(n, v)
            );

            // '/' <name>
            var flagArg = Rule(
                Match('/'),
                name,

                (s, n) => new FlagArgument(n)
            );

            // <named> | <flag> | <positional>
            var args = First<char, IParsedArgument>(
                namedArg,
                flagArg,
                value.Transform(v => new PositionalArgument(v))
            );

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}