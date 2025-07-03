using System.Collections.Generic;
using ParserObjects;
using StoneFruit.Execution.Scripts;
using StoneFruit.Execution.Scripts.Formatting;
using static ParserObjects.Parsers<char>;
using static ParserObjects.Parsers;
using static ParserObjects.Sequences;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Default ICommandParser implementation which controls parsing commands, arguments
/// and scripts.
/// </summary>
public class CommandParser : ICommandParser
{
    private readonly IParser<char, IReadOnlyList<ParsedArgument>> _argsParser;
    private readonly IParser<char, CommandFormat> _scriptParser;

    public CommandParser(IParser<char, ParsedArgument> argParser, IParser<char, CommandFormat> scriptParser)
    {
        NotNull(argParser);
        _argsParser = Rule(
            argParser.List(),
            OptionalWhitespace(),
            (args, _) => args
        );
        _scriptParser = NotNull(scriptParser);
    }

    /// <summary>
    /// Get the default CommandParser instance with default parser objects configured.
    /// </summary>
    /// <returns></returns>
    public static CommandParser GetDefault()
        => new CommandParser(
            SimplifiedArgumentGrammar.GetParser(),
            ScriptFormatGrammar.GetParser());

    /// <summary>
    /// Parse the given line of text as an IArguments.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public IArguments ParseCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            return SyntheticArguments.Empty();

        var sequence = FromString(command);
        var argsList = _argsParser.Parse(sequence).Value;
        if (sequence.IsAtEnd)
            return new ParsedArguments(argsList, command);

        var remainder = sequence.GetRemainder();
        throw new ParseException($"Could not parse all arguments. '{remainder}' fails at {sequence.CurrentLocation}");
    }

    /// <summary>
    /// Parse the given line of script as a CommandFormat to be used for creating commands.
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public CommandFormat ParseScript(string script)
    {
        var input = FromString(script);
        var parseResult = _scriptParser.Parse(input);
        if (!parseResult.Success)
            throw new ParseException($"Could not parse command format string: '{script}'");
        if (!input.IsAtEnd)
            throw new ParseException($"Parse did not complete for format string '{script}'. Unparsed remainder: '{input.GetRemainder()}'");
        return parseResult.Value;
    }
}
