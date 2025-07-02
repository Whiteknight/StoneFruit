using System;
using System.Linq;
using ParserObjects;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers<char>;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A grammar for posix-style arguments.
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
        ).Named("Value");

        // The name can be numbers or symbols or anything besides whitespace.
        var name = Match(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
            .ListCharToString(true)
            .Named("Name");

        var singleDash = Match('-');
        var doubleDash = Match("--");
        var singleIdChar = Match(char.IsLetterOrDigit);

        // '--' <name> '=' <value>
        var longEqualsNamedArg = Rule(
            doubleDash,
            name,
            Match('='),
            value,

            (_, n, _, v) => (IParsedArgument)new ParsedNamedArgument(n, v)
        ).Named("LongNameEqualsValue");

        // '--' <name> <ws> <value>
        var longImpliedNamedArg = Rule(
            doubleDash,
            name,
            whitespace,
            value,

            (_, n, _, v) => (IParsedArgument)new ParsedFlagPositionalOrNamedArgument(n, v)
        ).Named("LongNameImpliedValue");

        // '--' <name>
        var longFlagArg = Rule(
            doubleDash,
            name,

            (_, n) => (IParsedArgument)new ParsedFlagArgument(n)
        ).Named("LongFlag");

        // We include this case because some short args with a positional following, are treated like
        // a named arg. Provide all cases, because we don't know the intent.
        // '-' <char> <ws> <value>
        var shortImpliedNamedArg = Rule(
            singleDash,
            singleIdChar.Transform(c => c.ToString()),
            whitespace,
            value,

            (_, n, _, v) => (IParsedArgument)new ParsedFlagPositionalOrNamedArgument(n, v)
        ).Named("ShortNameImpliedValue");

        // '-' <char>*
        var shortFlagArg = Rule(
            singleDash,
            singleIdChar.List(true),

            (_, n) => (IParsedArgument)new MultiParsedFlagArgument(n.Select(x => x.ToString()))
        ).Named("ShortFlag");

        var positional = value
            .Transform(v => (IParsedArgument)new ParsedPositionalArgument(v))
            .Named("Positional");

        // <named> | <longFlag> | <shortFlag> | <positional>
        var args = First(
            longEqualsNamedArg,
            longImpliedNamedArg,
            longFlagArg,
            shortImpliedNamedArg,
            shortFlagArg,
            positional
        ).Named("Argument");

        return Rule(
            whitespace.Optional(),
            args,

            (_, arg) => arg
        ).Named("WhitespaceAndArgument");
    }
}
