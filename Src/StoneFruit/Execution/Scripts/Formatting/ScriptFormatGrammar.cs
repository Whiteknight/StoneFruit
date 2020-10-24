using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods<char>;
using static ParserObjects.Parsers.Specialty.CStyleParserMethods;
using static ParserObjects.Parsers.Specialty.QuotedParserMethods;
using static ParserObjects.Parsers.Specialty.WhitespaceParserMethods;

namespace StoneFruit.Execution.Scripts.Formatting
{
    public static class ScriptFormatGrammar
    {
        public static IParser<char, CommandFormat> CreateParser()
        {
            var doubleQuotedString = StrippedDoubleQuotedString();

            var singleQuotedString = StrippedSingleQuotedString();

            var names = Identifier();

            var integers = Integer();

            var whitespace = Whitespace().Optional();

            var quotedString = First(
                doubleQuotedString,
                singleQuotedString
            );

            var unquotedValue = Match(c => !char.IsWhiteSpace(c))
                .List(true)
                .Transform(c => new string(c.ToArray()));

            // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
            var values = First(
                doubleQuotedString,
                singleQuotedString,
                unquotedValue
            );

            var requiredOrDefaultValue = Rule(
                Match('!'),
                values.Optional(),

                (bang, v) => new RequiredValue(v)
            ).Optional();

            // A literal flag which is passed without modification
            // literalFlagArg := '-' <names>
            var literalFlagArg = Rule(
                Match('-'),
                names,

                (start, name) => new LiteralFlagArgumentAccessor(name)
            );

            // Fetch a flag from the input and rename it on the output if it exists
            // fetchFlagRenameArg := '?' <name> ':' <name>
            var fetchFlagRenameArg = Rule(
                Match('?'),
                names,
                Match(':'),
                names,

                (q, name, c, newName) => new FetchFlagArgumentAccessor(name, newName)
            );

            // Fetch a flag from the input and reproduce it on the output if it exists
            // fetchFlagArg := '?' <name>
            var fetchFlagArg = Rule(
                Match('?'),
                names,

                (q, name) => new FetchFlagArgumentAccessor(name)
            );

            // A literal named arg which is passed without modification
            // literalNamedArg := <name> '=' <value>
            var literalNamedArg = Rule(
                names,
                Match('='),
                values,

                (name, equals, value) => new LiteralNamedArgumentAccessor(name, value)
            );

            // A named argument where the name is a literal but the value fetched from a named arg
            // literalNameFetchValueArg := <name> '=' '[' <quotedString> ']' <requiredOrDefaultValue>
            var literalNameFetchNamedArg = Rule(
                names,
                Match('='),
                Match('['),
                values,
                Match(']'),
                requiredOrDefaultValue,

                (n, e, o, s, c, rdv) => new NamedFetchNamedArgumentAccessor(n, s, rdv != null, rdv?.DefaultValue)
            );

            // A named argument where the name is a literal but the value is fetched from a positional
            // literalNameFetchValueArg := <name> '=' '[' <integer> ']' <requiredOrDefaultValue>
            var literalNameFetchPositionalArg = Rule(
                names,
                Match('='),
                Match('['),
                integers,
                Match(']'),
                requiredOrDefaultValue,

                (n, e, o, i, c, rdv) => new NamedFetchPositionalArgumentAccessor(n, i, rdv != null, rdv?.DefaultValue)
            );

            // Fetch a named argument including name and value
            // "{b}" is equivalent to "b=['b']"
            // fetchNamedArg := '{' <unquotedValue> '}'
            var fetchNamedArg = Rule(
                Match('{'),
                names,
                Match('}'),
                requiredOrDefaultValue,

                (o, s, c, rdv) => new FetchNamedArgumentAccessor(s, rdv != null, rdv?.DefaultValue)
            );

            // Fetch all remaining unconsumed named arguments
            // fetchAllNamedArg := '{' '*' '}'
            var fetchAllNamedArg = Rule(
                Match('{'),
                Match('*'),
                Match('}'),

                (o, s, c) => new FetchAllNamedArgumentAccessor()
            );

            // A literal positional argument
            // literalPositionalArg := <values>
            var literalPositionalArg = values.Transform(v => new LiteralPositionalArgumentAccessor(v));

            // Fetch a positional argument by index
            // fetchPositionalArg := '[' <integer> ']'
            var fetchPositionalArg = Rule(
                Match('['),
                integers,
                Match(']'),
                requiredOrDefaultValue,

                (o, i, c, rdv) => new FetchPositionalArgumentAccessor(i, rdv != null, rdv?.DefaultValue)
            );

            // Fetch all remaining unconsumed positional arguments
            // fetchAllNamedArg := '[' '*' ']'
            var fetchAllPositionalsArg = Rule(
                Match('['),
                Match('*'),
                Match(']'),

                (o, s, c) => new FetchAllPositionalArgumentAccessor()
            );

            // Fetch all remaining unconsumed flag arguments
            // fetchAllFlagsArg := '-' '*'
            var fetchAllFlagsArg = Rule(
                Match('-'),
                Match('*'),

                (o, s) => new FetchAllFlagsArgumentAccessor()
            );

            // Fetch the value of a named argument and pass it as a positional argument
            // fetchNamedToPositionalArg := '[' <quotedString> ']'
            var fetchNamedToPositionalArg = Rule(
                Match('['),
                quotedString,
                Match(']'),
                requiredOrDefaultValue,

                (o, s, c, rdv) => new FetchNamedToPositionalArgumentAccessor(s, rdv != null, rdv?.DefaultValue)
            );

            // All possible args
            // <flag> | <named> | <positional>
            var args = First<IArgumentAccessor>(
                literalNameFetchNamedArg,
                literalNameFetchPositionalArg,
                literalNamedArg,
                fetchAllNamedArg,
                fetchNamedArg,

                fetchAllFlagsArg,
                literalFlagArg,
                fetchFlagRenameArg,
                fetchFlagArg,

                fetchAllPositionalsArg,
                fetchPositionalArg,
                fetchNamedToPositionalArg,
                literalPositionalArg
            );

            // An argument followed by optional whitespace
            var argAndWhitespace = Rule(
                args,
                whitespace,

                (a, ws) => a
            );

            // The command with verb and all arguments
            // command := <verb> <argAndWhitespace>* <end>
            return Rule(
                argAndWhitespace.List(true),
                End(),

                (a, end) => new CommandFormat(a.ToList())
            );
        }

        // If this object exists at all, the preceeding argument is required. It may have a default
        // value if one is specified
        private class RequiredValue
        {
            public RequiredValue(string defaultValue)
            {
                DefaultValue = defaultValue;
            }

            public string DefaultValue { get; }
        }
    }
}