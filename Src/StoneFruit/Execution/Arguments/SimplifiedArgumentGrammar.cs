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
    /// A grammar for simplified arguments
    /// </summary>
    public class SimplifiedArgumentGrammar
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

                (start, name) => new FlagArgument(name)
            );

            // <name> '=' <value>
            var namedArg = Rule(
                names,
                Match('='),
                values,

                (name, equals, value) => new NamedArgument(name, value) 
            );

            // <flag> | <named> | <positional>
            var args = First<char, IArgument>(
                flagArg,
                namedArg,
                values.Transform(v => new PositionalArgument(v) )
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