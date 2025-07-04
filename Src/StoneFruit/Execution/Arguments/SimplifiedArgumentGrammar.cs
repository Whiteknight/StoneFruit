using System;
using ParserObjects;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers.C;
using static ParserObjects.Parsers<char>;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A grammar for simplified arguments.
/// </summary>
public static class SimplifiedArgumentGrammar
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

        // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
        var values = First(
            doubleQuotedString,
            singleQuotedString,
            unquotedValue
        ).Named("Value");

        var names = Identifier();

        // '-' <name>
        var flagArg = Rule(
            Match('-'),
            names,

            (_, name) => (ArgumentToken)new ParsedFlag(name)
        ).Named("Flag");

        // <name> '=' <value>
        var namedArg = Rule(
            names,
            Match('='),
            values,

            (name, _, value) => (ArgumentToken)new ParsedNamed(name, value)
        ).Named("Named");

        // <flag> | <named> | <positional>
        var args = First<ArgumentToken>(
            flagArg,
            namedArg,
            values.Transform(v => (ArgumentToken)new ParsedPositional(v))
        ).Named("Argument");

        return Rule(
            OptionalWhitespace(),
            args,

            (_, arg) => arg
        ).Named("WhitespaceAndArgument");
    }
}
