using System;
using System.Linq;
using ParserObjects;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers.C;
using static ParserObjects.Parsers<char>;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A grammar for Powershell-style arguments.
/// </summary>
public static class PowershellStyleArgumentGrammar
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

        // '--' | '-'
        var nameStart = First(
            Match("--").Transform(_ => '-'),
            Match('-')
        );

        // <doubleQuotedString> | <singleQuotedString> | <unquotedValue>
        var values = First(
            doubleQuotedString,
            singleQuotedString,
            unquotedValue
        ).Named("Value");

        var names = Identifier();

        // Powershell convention doesn't really have a clear way to specify that a switch/value is a
        // named arg or just a switch followed by a positional. Return a combined argument which acts
        // like all three and the user can consume it however they want

        // <nameStart> <name> <whitespace> <value>
        var namedArg = Rule(
            nameStart,
            names,
            whitespace,
            values,

            (_, name, _, value) => new ParsedFlagAndPositionalOrNamed(name, value)
        ).Named("Named");

        // <nameStart> <name>
        var longFlagArg = Rule(
            nameStart,
            names,

            (_, name) => new ParsedFlag(name)
        ).Named("LongFlag");

        // <named> | <longFlag> | <positional>
        var args = First<ParsedArgument>(
            namedArg,
            longFlagArg,
            values.Transform(v => new ParsedPositional(v))
        ).Named("Argument");

        var whitespaceAndArgs = Rule(
            whitespace,
            args,

            (_, arg) => arg
        ).Named("WhitespaceAndArgument");

        return whitespaceAndArgs;
    }
}
