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
    private static readonly Lazy<IParser<char, IArgumentToken>> _instance
        = new Lazy<IParser<char, IArgumentToken>>(GetParserInternal);

    public static IParser<char, IArgumentToken> GetParser() => _instance.Value;

    private static IParser<char, IArgumentToken> GetParserInternal()
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
            (_, name) => (IArgumentToken)new ParsedFlag(name)
        ).Named("Flag");

        // <name> '=' <value>
        var namedArg = Rule(
            names,
            Match('='),
            values,
            (name, _, value) => (IArgumentToken)new ParsedNamed(name, value)
        ).Named("Named");

        // <flag> | <named> | <positional>
        var args = First<IArgumentToken>(
            flagArg,
            namedArg,
            values.Transform(v => (IArgumentToken)new ParsedPositional(v))
        ).Named("Argument");

        return Rule(
            OptionalWhitespace(),
            args,
            (_, arg) => arg
        ).Named("WhitespaceAndArgument");
    }
}
