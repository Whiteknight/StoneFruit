using System;
using ParserObjects;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers.C;
using static ParserObjects.Parsers<char>;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// An argument grammar for windows CMD-style arguments.
/// </summary>
public static class WindowsCmdArgumentGrammar
{
    private static readonly Lazy<IParser<char, IArgumentToken>> _instance
        = new Lazy<IParser<char, IArgumentToken>>(GetParserInternal);

    public static IParser<char, IArgumentToken> GetParser() => _instance.Value;

    private static IParser<char, IArgumentToken> GetParserInternal()
    {
        var doubleQuotedString = StrippedDoubleQuotedString();

        var singleQuotedString = StrippedSingleQuotedString();

        var unquotedValue = Match(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c))
            .ListCharToString(true);

        var whitespace = OptionalWhitespace();

        var value = First(
            doubleQuotedString,
            singleQuotedString,
            unquotedValue
        ).Named("Value");

        var name = Identifier();

        // '/' <name> ':' <value>
        var namedArg = Rule(
            Match('/'),
            name,
            Match(':'),
            value,
            (_, n, _, v) => (IArgumentToken)new ParsedNamed(n, v)
        ).Named("Named");

        // '-' <name> <whitespace> <value>
        var maybeNamedArg = Rule(
            Match('/'),
            name,
            whitespace,
            value,
            (_, n, _, v) => (IArgumentToken)new ParsedFlagAndPositionalOrNamed(n, v)
        ).Named("NamedMaybeValue");

        // '/' <name>
        var flagArg = Rule(
            Match('/'),
            name,
            (_, n) => (IArgumentToken)new ParsedFlag(n)
        ).Named("Flag");

        // <named> | <flag> | <positional>
        var args = First<IArgumentToken>(
            namedArg,
            maybeNamedArg,
            flagArg,
            value.Transform(v => (IArgumentToken)new ParsedPositional(v))
        ).Named("Argument");

        return Rule(
            whitespace,
            args,
            (_, arg) => arg
        );
    }
}
