﻿using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.Specialty.ProgrammingParserMethods;
using static ParserObjects.Parsers.Specialty.ParserMethods;

namespace StoneFruit.Execution.Arguments
{
    public class SimplifiedArgumentGrammar
    {
        public static IParser<char, IEnumerable<IArgument>> GetParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedStringWithEscapedQuotes();
            var singleQuotedString = StrippedSingleQuotedStringWithEscapedQuotes();
            var unquotedValue = Match<char>(c => !char.IsWhiteSpace(c)).List(c => new string(c.ToArray()), true);

            var value = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var name = CStyleIdentifier();

            var namedArg = Rule(
                name,
                Match("=", c => c),
                value,

                (n, e, v) => new IArgument[] { new NamedArgument(n, v) }
            );

            var args = First<char, IEnumerable<IArgument>>(
                namedArg,
                value.Transform(v => new [] { new PositionalArgument(v) })
            );

            var whitespace = Whitespace();

            return Rule(
                whitespace,
                args,

                (ws, arg) => arg
            );
        }
    }
}