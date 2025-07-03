using System;
using System.Linq;
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
    private static readonly Lazy<IParser<char, ParsedArgument>> _instance
        = new Lazy<IParser<char, ParsedArgument>>(GetParserInternal);

    public static IParser<char, ParsedArgument> GetParser() => _instance.Value;

    private static IParser<char, ParsedArgument> GetParserInternal()
    {
        var doubleQuotedString = StrippedDoubleQuotedString();

        var singleQuotedString = StrippedSingleQuotedString();

        var unquotedValue = Match(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c))
            .List(true)
            .Transform(c => new string(c.ToArray()));

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

            (_, n, _, v) => new ParsedNamed(n, v)
        ).Named("Named");

        // '-' <name> <whitespace> <value>
        var maybeNamedArg = Rule(
            Match('/'),
            name,
            whitespace,
            value,

            (_, name, _, value) => new ParsedFlagAndPositionalOrNamed(name, value)
        ).Named("NamedMaybeValue");

        // '/' <name>
        var flagArg = Rule(
            Match('/'),
            name,

            (_, n) => new ParsedFlag(n)
        ).Named("Flag");

        // <named> | <flag> | <positional>
        var args = First<ParsedArgument>(
            namedArg,
            maybeNamedArg,
            flagArg,
            value.Transform(v => new ParsedPositional(v))
        ).Named("Argument");

        return Rule(
            whitespace,
            args,

            (_, arg) => arg
        );
    }
}
