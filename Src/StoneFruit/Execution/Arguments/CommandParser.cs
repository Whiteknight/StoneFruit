using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Scripts;
using static ParserObjects.Parsers;
using static ParserObjects.Parsers<char>;
using static ParserObjects.Sequences;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Default ICommandParser implementation which controls parsing commands, arguments
/// and scripts.
/// </summary>
public class CommandParser : ICommandParser
{
    private readonly IParser<char, IReadOnlyList<ArgumentToken>> _argsParser;
    private readonly IParser<char, CommandFormat> _scriptParser;

    public CommandParser(IParser<char, ArgumentToken> argParser, IParser<char, CommandFormat> scriptParser)
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

    public Result<List<ArgumentsOrString>, ScriptsError> ParseScript(IReadOnlyList<string> lines, IArguments args)
    {
        var x = lines
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(ParseScriptLine)
            .Select((format, i) => format.Format(args, i))
            .Aggregate(CommandList.New(), (l, r) => r.Match(
                success => l.Add(success),
                errors => l.Add(errors)));
        if (x.Errors.Count != 0)
            return x.Errors.Aggregate((e1, e2) => e1.Combine(e2));

        return x.Arguments.ConvertAll(l => new ArgumentsOrString(l));
    }

    private CommandFormat ParseScriptLine(string script)
    {
        var input = FromString(script);
        var parseResult = _scriptParser.Parse(input);
        if (!parseResult.Success)
            throw new ParseException($"Could not parse command format string: '{script}': {parseResult.ErrorMessage}");
        if (!input.IsAtEnd)
            throw new ParseException($"Parse did not complete for format string '{script}'. Unparsed remainder: '{input.GetRemainder()}'");
        return parseResult.Value;
    }

    private readonly record struct CommandList(List<IArguments> Arguments, List<ScriptsError> Errors)
    {
        public static CommandList New() => new CommandList([], []);

        public CommandList Add(IReadOnlyList<IArgument> s)
        {
            var args = s.ToSyntheticArguments();
            args.Reset();
            Arguments.Add(args);
            return this;
        }

        public CommandList Add(ScriptsError error)
        {
            Errors.Add(error);
            return this;
        }
    }
}
