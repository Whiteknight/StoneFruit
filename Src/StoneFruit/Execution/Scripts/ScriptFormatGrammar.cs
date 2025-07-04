using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers.C;
using static ParserObjects.Parsers<char>;

namespace StoneFruit.Execution.Scripts;

public static class ScriptFormatGrammar
{
    private static readonly Lazy<IParser<char, CommandFormat>> _instance = new Lazy<IParser<char, CommandFormat>>(GetParserInternal);

    public static IParser<char, CommandFormat> GetParser() => _instance.Value;

    private static IParser<char, CommandFormat> GetParserInternal()
    {
        var doubleQuotedString = StrippedDoubleQuotedString();

        var singleQuotedString = StrippedSingleQuotedString();

        var names = Identifier();

        var integers = Integer();

        var quotedString = First(
            doubleQuotedString,
            singleQuotedString
        );

        var unquotedValue = Match(c => c != ':' && (char.IsLetterOrDigit(c) || char.IsPunctuation(c)))
            .ListCharToString(true);

        // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
        var values = First(
            doubleQuotedString,
            singleQuotedString,
            unquotedValue
        );

        // Default values get wedged into [] or {} brackets, or at the end of a flag name, so they are more
        // restricted in terms of values they can take without quoting.
        var unquotedDefaultValue = Match(c => char.IsLetterOrDigit(c) || (char.IsPunctuation(c) && c != ']' && c != '}'))
            .ListCharToString(true);

        var possibleDefaultValue = First(
            doubleQuotedString,
            singleQuotedString,
            unquotedDefaultValue
        );

        var optionalDefaultValue = Rule(
           Match(':'),
           possibleDefaultValue,
           (_, value) => value
       ).Optional();

        var maybeRequired = First(
            Match('!').Transform(_ => true),
            Produce(() => false)
        );

        // We cannot have a * syntax for all remaining unconsumed args, because some IArguments
        // like ParsedArguments contain ambiguities, and we can't just consume an arg without
        // knowing whether it is a positional, named, or flag. If you want all, you can append
        // "-* {*} [*]" to the end of your pattern, or in whatever order you want ambiguities
        // to be resolved in.

        // A literal flag which is passed without modification
        // literalFlagArg := '-' <names>
        var literalFlagArg = Rule(
            Match('-'),
            names,

            (_, name) => new LiteralFlagArgumentAccessor(name)
        );

        // Fetch a flag from the input and rename it on the output if it exists
        // It doesn't make sense for a flag to be required, so we don't support that syntax here
        // fetchFlagArg := '?' <name> (':' <name>)?
        var fetchFlagArg = Rule(
            Match('?'),
            names,
            Rule(
                Match(':'),
                names,
                (_, newName) => newName
            ).Optional(),

            (_, name, newName) => new FetchFlagArgumentAccessor(name, newName)
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
            quotedString,
            optionalDefaultValue,
            Match(']'),
            maybeRequired,

            (n, _, _, s, defaultValue, _, required) => new NamedFetchNamedArgumentAccessor(n, s, required, defaultValue)
        );

        // A named argument where the name is a literal but the value is fetched from a positional
        // literalNameFetchValueArg := <name> '=' '[' <integer> ']' <requiredOrDefaultValue>
        var literalNameFetchPositionalArg = Rule(
            names,
            Match('='),
            Match('['),
            integers,
            optionalDefaultValue,
            Match(']'),
            maybeRequired,

            (n, _, _, i, defaultValue, _, required) => new NamedFetchPositionalArgumentAccessor(n, i, required, defaultValue)
        );

        // Fetch a named argument including name and value
        // "{b}" is equivalent to "b=['b']"
        // fetchNamedArg := '{' <unquotedValue> '}'
        var fetchNamedArg = Rule(
            Match('{'),
            names,
            optionalDefaultValue,
            Match('}'),
            maybeRequired,

            (_, s, defaultValue, _, required) => new FetchNamedArgumentAccessor(s, required, defaultValue)
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
            optionalDefaultValue,
            Match(']'),
            maybeRequired,

            (_, i, defaultValue, _, required) => new FetchPositionalArgumentAccessor(i, required, defaultValue)
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
            optionalDefaultValue,
            Match(']'),
            maybeRequired,

            (_, s, defaultValue, _, required) => new FetchNamedToPositionalArgumentAccessor(s, required, defaultValue)
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
            fetchFlagArg,

            fetchAllPositionalsArg,
            fetchPositionalArg,
            fetchNamedToPositionalArg,
            literalPositionalArg
        );

        // The command with verb and all arguments
        // command := <verb> <argAndWhitespace>* <end>
        return Rule(
            args.List(Whitespace(), true),
            If(End(), Produce(() => true)),

            (a, _) => new CommandFormat(a)
        );
    }

    private class FetchAllFlagsArgumentAccessor : IArgumentAccessor
    {
        public IEnumerable<IArgument> Access(IArguments args)
            => args.GetAllFlags()
                .Tap(f => f.MarkConsumed())
                .Select(f => new FlagArgument(f.Name))
                .ToList();
    }

    private class FetchAllNamedArgumentAccessor : IArgumentAccessor
    {
        public IEnumerable<IArgument> Access(IArguments args)
            => args.GetAllNamed()
                .Tap(n => n.MarkConsumed())
                .Select(n => new NamedArgument(n.Name, n.Value))
                .ToList();
    }

    private class FetchAllPositionalArgumentAccessor : IArgumentAccessor
    {
        public IEnumerable<IArgument> Access(IArguments args)
            => args.GetAllPositionals()
                .Tap(p => p.MarkConsumed())
                .Select(p => new PositionalArgument(p.Value))
                .ToList();
    }

    private class FetchFlagArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;
        private readonly Option<string> _newName;

        public FetchFlagArgumentAccessor(string name, Option<string> newName)
        {
            _name = name;
            _newName = newName;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            var flag = args.GetFlag(_name);
            if (!flag.Exists())
                return [];
            flag.MarkConsumed();
            var name = _newName.GetValueOrDefault(_name);
            return [new FlagArgument(name)];
        }
    }

    private class FetchNamedArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;
        private readonly bool _required;
        private readonly Option<string> _defaultValue;

        public FetchNamedArgumentAccessor(string name, bool required, Option<string> defaultValue)
        {
            _name = name;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            // See if we have the requested value
            var arg = args.Get(_name);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return [new NamedArgument(_name, arg.AsString(string.Empty))];
            }

            // See if we have a default value
            if (_defaultValue.Success)
                return [new NamedArgument(_name, _defaultValue.Value)];

            // See if we can ignore it
            if (!_required)
                return [];

            // We're missing a required value
            throw ArgumentParseException.MissingRequiredArgument(_name);
        }
    }

    private class FetchNamedToPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly string _name;
        private readonly bool _required;
        private readonly Option<string> _defaultValue;

        public FetchNamedToPositionalArgumentAccessor(string name, bool required, Option<string> defaultValue)
        {
            _name = name;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            // See if we have the argument
            var arg = args.Get(_name);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return [new PositionalArgument(arg.AsString(string.Empty))];
            }

            // See if we have a default value
            if (_defaultValue.Success)
                return [new PositionalArgument(_defaultValue.Value)];

            // See if it's optional
            if (!_required)
                return [];

            // We're missing a required argument
            throw ArgumentParseException.MissingRequiredArgument(_name);
        }
    }

    private class FetchPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly int _index;
        private readonly bool _required;
        private readonly Option<string> _defaultValue;

        public FetchPositionalArgumentAccessor(int index, bool required, Option<string> defaultValue)
        {
            _index = index;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            // See if we have the value
            var arg = args.Get(_index);
            if (arg.Exists() && !arg.Consumed)
            {
                arg.MarkConsumed();
                return [new PositionalArgument(arg.AsString(string.Empty))];
            }

            // See if we have a default value
            if (_defaultValue.Success)
                return [new PositionalArgument(_defaultValue.Value)];

            // See if it's optional
            if (!_required)
                return [];

            // We're missing a required value
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
            => [new FlagArgument(_name)];
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
            => [new NamedArgument(_name, _value)];
    }

    private class LiteralPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly string _value;

        public LiteralPositionalArgumentAccessor(string value)
        {
            _value = value;
        }

        public IEnumerable<IArgument> Access(IArguments args)
            => [new PositionalArgument(_value)];
    }

    private class NamedFetchNamedArgumentAccessor : IArgumentAccessor
    {
        private readonly string _newName;
        private readonly string _oldName;
        private readonly bool _required;
        private readonly Option<string> _defaultValue;

        public NamedFetchNamedArgumentAccessor(string newName, string oldName, bool required, Option<string> defaultValue)
        {
            _newName = newName;
            _oldName = oldName;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            // First, see if we have the value
            var arg = args.Get(_oldName);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return [new NamedArgument(_newName, arg.AsString(string.Empty)),];
            }

            // Second see if we have a default value
            if (_defaultValue.Success)
                return [new NamedArgument(_newName, _defaultValue.Value)];

            // Third, if this value isn't required, return nothing
            if (!_required)
                return [];

            // Throw an exception, we're missing something that's required.
            throw ArgumentParseException.MissingRequiredArgument(_oldName);
        }
    }

    private class NamedFetchPositionalArgumentAccessor : IArgumentAccessor
    {
        private readonly string _newName;
        private readonly int _index;
        private readonly bool _required;
        private readonly Option<string> _defaultValue;

        public NamedFetchPositionalArgumentAccessor(string newName, int index, bool required, Option<string> defaultValue)
        {
            _newName = newName;
            _index = index;
            _required = required;
            _defaultValue = defaultValue;
        }

        public IEnumerable<IArgument> Access(IArguments args)
        {
            // See if we have the requested value
            var arg = args.Get(_index);
            if (arg.Exists())
            {
                arg.MarkConsumed();
                return [new NamedArgument(_newName, arg.AsString(string.Empty))];
            }

            // See if we have a default value
            if (_defaultValue.Success)
                return [new NamedArgument(_newName, _defaultValue.Value)];

            // See if we can ignore it
            if (!_required)
                return [];

            // Throw an error that we're missing a required value
            throw ArgumentParseException.MissingRequiredArgument(_index);
        }
    }
}
