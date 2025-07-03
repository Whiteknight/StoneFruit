using System;
using System.Linq;
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

            (_, name) => new ParsedFlag(name)
        ).Named("Flag");

        // <name> '=' <value>
        var namedArg = Rule(
            names,
            Match('='),
            values,

            (name, _, value) => new ParsedNamed(name, value)
        ).Named("Named");

        // <flag> | <named> | <positional>
        var args = First<ParsedArgument>(
            flagArg,
            namedArg,
            values.Transform(v => new ParsedPositional(v))
        ).Named("Argument");

        return Rule(
            OptionalWhitespace(),
            args,

            (_, arg) => arg
        ).Named("WhitespaceAndArgument");
    }
}
