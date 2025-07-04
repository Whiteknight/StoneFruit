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
    private static readonly Lazy<IParser<char, ArgumentToken>> _instance
        = new Lazy<IParser<char, ArgumentToken>>(GetParserInternal);

    public static IParser<char, ArgumentToken> GetParser() => _instance.Value;

    private static IParser<char, ArgumentToken> GetParserInternal()
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

            (_, n, _, v) => (ArgumentToken)new ParsedNamed(n, v)
        ).Named("Named");

        // '-' <name> <whitespace> <value>
        var maybeNamedArg = Rule(
            Match('/'),
            name,
            whitespace,
            value,

            (_, n, _, v) => (ArgumentToken)new ParsedFlagAndPositionalOrNamed(n, v)
        ).Named("NamedMaybeValue");

        // '/' <name>
        var flagArg = Rule(
            Match('/'),
            name,

            (_, n) => (ArgumentToken)new ParsedFlag(n)
        ).Named("Flag");

        // <named> | <flag> | <positional>
        var args = First<ArgumentToken>(
            namedArg,
            maybeNamedArg,
            flagArg,
            value.Transform(v => (ArgumentToken)new ParsedPositional(v))
        ).Named("Argument");

        return Rule(
            whitespace,
            args,

            (_, arg) => arg
        );
    }
}
