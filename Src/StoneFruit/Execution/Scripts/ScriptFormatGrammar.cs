using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts.Formatting;
using static ParserObjects.CStyleParserMethods;
using static ParserObjects.ParserMethods;
using static ParserObjects.ParserMethods<char>;

namespace StoneFruit.Execution.Scripts
{
    public static class ScriptFormatGrammar
    {
        private static readonly RequiredValue _noRequiredValue = new RequiredValue("");
        private static readonly Lazy<IParser<char, CommandFormat>> _instance = new Lazy<IParser<char, CommandFormat>>(GetParserInternal);

        public static IParser<char, CommandFormat> GetParser() => _instance.Value;

        private static IParser<char, CommandFormat> GetParserInternal()
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

            var unquotedValue = Match(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c))
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

                (_, v) => new RequiredValue(v.GetValueOrDefault(""))
            ).Optional();

            // A literal flag which is passed without modification
            // literalFlagArg := '-' <names>
            var literalFlagArg = Rule(
                Match('-'),
                names,

                (_, name) => new LiteralFlagArgumentAccessor(name)
            );

            // Fetch a flag from the input and rename it on the output if it exists
            // fetchFlagRenameArg := '?' <name> ':' <name>
            var fetchFlagRenameArg = Rule(
                Match('?'),
                names,
                Match(':'),
                names,

                (_, name, _, newName) => new FetchFlagArgumentAccessor(name, newName)
            );

            // Fetch a flag from the input and reproduce it on the output if it exists
            // fetchFlagArg := '?' <name>
            var fetchFlagArg = Rule(
                Match('?'),
                names,

                (_, name) => new FetchFlagArgumentAccessor(name, name)
            );

            // A literal named arg which is passed without modification
            // literalNamedArg := <name> '=' <value>
            var literalNamedArg = Rule(
                names,
                Match('='),
                values,

                (name, _, value) => new LiteralNamedArgumentAccessor(name, value)
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

                (n, _, _, s, _, rdv) => new NamedFetchNamedArgumentAccessor(n, s, rdv.Success, rdv.GetValueOrDefault(_noRequiredValue).DefaultValue)
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

                (n, _, _, i, _, rdv) => new NamedFetchPositionalArgumentAccessor(n, i, rdv.Success, rdv.GetValueOrDefault(_noRequiredValue).DefaultValue)
            );

            // Fetch a named argument including name and value
            // "{b}" is equivalent to "b=['b']"
            // fetchNamedArg := '{' <unquotedValue> '}'
            var fetchNamedArg = Rule(
                Match('{'),
                names,
                Match('}'),
                requiredOrDefaultValue,

                (_, s, _, rdv) => new FetchNamedArgumentAccessor(s, rdv.Success, rdv.GetValueOrDefault(_noRequiredValue).DefaultValue)
            );

            // Fetch all remaining unconsumed named arguments
            // fetchAllNamedArg := '{' '*' '}'
            var fetchAllNamedArg = Rule(
                Match('{'),
                Match('*'),
                Match('}'),

                (_, _, _) => new FetchAllNamedArgumentAccessor()
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

                (_, i, _, rdv) => new FetchPositionalArgumentAccessor(i, rdv.Success, rdv.GetValueOrDefault(_noRequiredValue).DefaultValue)
            );

            // Fetch all remaining unconsumed positional arguments
            // fetchAllNamedArg := '[' '*' ']'
            var fetchAllPositionalsArg = Rule(
                Match('['),
                Match('*'),
                Match(']'),

                (_, _, _) => new FetchAllPositionalArgumentAccessor()
            );

            // Fetch all remaining unconsumed flag arguments
            // fetchAllFlagsArg := '-' '*'
            var fetchAllFlagsArg = Rule(
                Match('-'),
                Match('*'),

                (_, _) => new FetchAllFlagsArgumentAccessor()
            );

            // Fetch the value of a named argument and pass it as a positional argument
            // fetchNamedToPositionalArg := '[' <quotedString> ']'
            var fetchNamedToPositionalArg = Rule(
                Match('['),
                quotedString,
                Match(']'),
                requiredOrDefaultValue,

                (_, s, _, rdv) => new FetchNamedToPositionalArgumentAccessor(s, rdv.Success, rdv.GetValueOrDefault(_noRequiredValue).DefaultValue)
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

                (a, _) => a
            );

            // The command with verb and all arguments
            // command := <verb> <argAndWhitespace>* <end>
            return Rule(
                argAndWhitespace.List(true),
                If(End(), Produce(() => true)),

                (a, _) => new CommandFormat(a.ToList())
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

        private class FetchAllFlagsArgumentAccessor : IArgumentAccessor
        {
            public IEnumerable<IArgument> Access(IArguments args)
            {
                var results = new List<IArgument>();
                var flags = args.GetAllFlags();
                foreach (var flag in flags)
                {
                    flag.MarkConsumed();
                    results.Add(new FlagArgument(flag.Name));
                }

                return results;
            }
        }

        private class FetchAllNamedArgumentAccessor : IArgumentAccessor
        {
            public IEnumerable<IArgument> Access(IArguments args)
            {
                var results = new List<IArgument>();
                foreach (var named in args.GetAllNamed())
                {
                    named.MarkConsumed();
                    results.Add(new NamedArgument(named.Name, named.Value));
                }

                return results;
            }
        }

        private class FetchAllPositionalArgumentAccessor : IArgumentAccessor
        {
            public IEnumerable<IArgument> Access(IArguments args)
            {
                var results = new List<IArgument>();
                foreach (var positional in args.GetAllPositionals())
                {
                    positional.MarkConsumed();
                    results.Add(new PositionalArgument(positional.Value));
                }
                return results;
            }
        }

        private class FetchFlagArgumentAccessor : IArgumentAccessor
        {
            private readonly string _name;
            private readonly string _newName;

            public FetchFlagArgumentAccessor(string name, string newName)
            {
                _name = name;
                _newName = newName;
            }

            public IEnumerable<IArgument> Access(IArguments args)
            {
                var flag = args.GetFlag(_name);
                if (!flag.Exists())
                    return Enumerable.Empty<IArgument>();
                flag.MarkConsumed();
                return new[] { new FlagArgument(_newName) };
            }
        }

        private class FetchNamedArgumentAccessor : IArgumentAccessor
        {
            private readonly string _name;
            private readonly bool _required;
            private readonly string _defaultValue;

            public FetchNamedArgumentAccessor(string name, bool required, string defaultValue)
            {
                _name = name;
                _required = required;
                _defaultValue = defaultValue;
            }

            public IEnumerable<IArgument> Access(IArguments args)
            {
                var arg = args.Get(_name);
                if (arg.Exists())
                {
                    arg.MarkConsumed();
                    return new[] { new NamedArgument(_name, arg.AsString(string.Empty)) };
                }
                if (!_required)
                    return Enumerable.Empty<IArgument>();
                if (!string.IsNullOrEmpty(_defaultValue))
                    return new[] { new NamedArgument(_name, _defaultValue) };
                throw ArgumentParseException.MissingRequiredArgument(_name);
            }
        }

        private class FetchNamedToPositionalArgumentAccessor : IArgumentAccessor
        {
            private readonly string _name;
            private readonly bool _required;
            private readonly string _defaultValue;

            public FetchNamedToPositionalArgumentAccessor(string name, bool required, string defaultValue)
            {
                _name = name;
                _required = required;
                _defaultValue = defaultValue;
            }

            public IEnumerable<IArgument> Access(IArguments args)
            {
                var arg = args.Get(_name);
                if (arg.Exists())
                {
                    arg.MarkConsumed();
                    return new[] { new PositionalArgument(arg.AsString(string.Empty)) };
                }
                if (!_required)
                    return Enumerable.Empty<IArgument>();
                if (!string.IsNullOrEmpty(_defaultValue))
                    return new[] { new PositionalArgument(_defaultValue) };

                throw ArgumentParseException.MissingRequiredArgument(_name);
            }
        }

        private class FetchPositionalArgumentAccessor : IArgumentAccessor
        {
            private readonly int _index;
            private readonly bool _required;
            private readonly string _defaultValue;

            public FetchPositionalArgumentAccessor(int index, bool required, string defaultValue)
            {
                _index = index;
                _required = required;
                _defaultValue = defaultValue;
            }

            public IEnumerable<IArgument> Access(IArguments args)
            {
                var arg = args.Get(_index);
                if (arg.Exists() && !arg.Consumed)
                {
                    arg.MarkConsumed();
                    return new[] { new PositionalArgument(arg.AsString(string.Empty)) };
                }
                if (!_required)
                    return Enumerable.Empty<IArgument>();
                if (!string.IsNullOrEmpty(_defaultValue))
                    return new[] { new PositionalArgument(_defaultValue) };

                throw ArgumentParseException.MissingRequiredArgument(_index);
            }
        }

        private class LiteralFlagArgumentAccessor : IArgumentAccessor
        {
            private readonly string _name;

            public LiteralFlagArgumentAccessor(string name)
            {
                _name = name;
            }

            public IEnumerable<IArgument> Access(IArguments args)
                => new[] { new FlagArgument(_name) };
        }

        private class LiteralNamedArgumentAccessor : IArgumentAccessor
        {
            private readonly string _name;
            private readonly string _value;

            public LiteralNamedArgumentAccessor(string name, string value)
            {
                _name = name;
                _value = value;
            }

            public IEnumerable<IArgument> Access(IArguments args)
                => new[] { new NamedArgument(_name, _value) };
        }

        private class LiteralPositionalArgumentAccessor : IArgumentAccessor
        {
            private readonly string _value;

            public LiteralPositionalArgumentAccessor(string value)
            {
                _value = value;
            }

            public IEnumerable<IArgument> Access(IArguments args)
                => new[] { new PositionalArgument(_value) };
        }

        private class NamedFetchNamedArgumentAccessor : IArgumentAccessor
        {
            private readonly string _newName;
            private readonly string _oldName;
            private readonly bool _required;
            private readonly string _defaultValue;

            public NamedFetchNamedArgumentAccessor(string newName, string oldName, bool required, string defaultValue)
            {
                _newName = newName;
                _oldName = oldName;
                _required = required;
                _defaultValue = defaultValue;
            }

            public IEnumerable<IArgument> Access(IArguments args)
            {
                var arg = args.Get(_oldName);
                if (arg.Exists())
                {
                    arg.MarkConsumed();
                    return new[] { new NamedArgument(_newName, arg.AsString(string.Empty)), };
                }
                if (!_required)
                    return Enumerable.Empty<IArgument>();
                if (!string.IsNullOrEmpty(_defaultValue))
                    return new[] { new NamedArgument(_newName, _defaultValue) };
                throw ArgumentParseException.MissingRequiredArgument(_oldName);
            }
        }

        private class NamedFetchPositionalArgumentAccessor : IArgumentAccessor
        {
            private readonly string _newName;
            private readonly int _index;
            private readonly bool _required;
            private readonly string _defaultValue;

            public NamedFetchPositionalArgumentAccessor(string newName, int index, bool required, string defaultValue)
            {
                _newName = newName;
                _index = index;
                _required = required;
                _defaultValue = defaultValue;
            }

            public IEnumerable<IArgument> Access(IArguments args)
            {
                var arg = args.Get(_index);
                if (arg.Exists())
                {
                    arg.MarkConsumed();
                    return new[] { new NamedArgument(_newName, arg.AsString(string.Empty)) };
                }
                if (!_required)
                    return Enumerable.Empty<IArgument>();
                if (!string.IsNullOrEmpty(_defaultValue))
                    return new[] { new NamedArgument(_newName, _defaultValue) };
                throw ArgumentParseException.MissingRequiredArgument(_index);
            }
        }
    }
}
