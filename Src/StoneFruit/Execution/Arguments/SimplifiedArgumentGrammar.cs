﻿using System;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods<char>;
using static ParserObjects.Parsers.Specialty.CStyleParserMethods;
using static ParserObjects.Parsers.Specialty.QuotedParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;

namespace StoneFruit.Execution.Arguments
{
    /// <summary>
    /// A grammar for simplified arguments
    /// </summary>
    public static class SimplifiedArgumentGrammar
    {
        private static readonly Lazy<IParser<char, IParsedArgument>> _instance = new Lazy<IParser<char, IParsedArgument>>(GetParserInternal);

        public static IParser<char, IParsedArgument> GetParser() => _instance.Value;

        private static IParser<char, IParsedArgument> GetParserInternal()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var unquotedValue = Match(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var names = Identifier();

            // '-' <name>
            var flagArg = Rule(
                Match('-'),
                names,

                (start, name) => new ParsedFlagArgument(name)
            );

            // <name> '=' <value>
            var namedArg = Rule(
                names,
                Match('='),
                values,

                (name, equals, value) => new ParsedNamedArgument(name, value)
            );

            // <flag> | <named> | <positional>
            var args = First<IParsedArgument>(
                flagArg,
                namedArg,
                values.Transform(v => new ParsedPositionalArgument(v))
            );

            var whitespace = Whitespace();

            return Rule(
                whitespace.Optional(),
                args,

                (ws, arg) => arg
            );
        }
    }
}