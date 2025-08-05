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
    private readonly ArgumentValueMapper _mapper;

    public CommandParser(IParser<char, ArgumentToken> argParser, IParser<char, CommandFormat> scriptParser, ArgumentValueMapper mapper)
    {
        NotNull(argParser);
        _argsParser = Rule(
            argParser.List(),
            OptionalWhitespace(),
            (args, _) => args
        );
        _scriptParser = NotNull(scriptParser);
        _mapper = NotNull(mapper);
    }

    /// <summary>
    /// Parse the given line of text as an IArguments.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public IArguments ParseCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            return Arguments.Empty;

        var sequence = FromString(command);
        var argsList = _argsParser.Parse(sequence).Value;
        if (sequence.IsAtEnd)
            return new Arguments(new ParsedArguments(argsList, command), _mapper);

        var remainder = sequence.GetRemainder();
        throw new ParseException($"Could not parse all arguments. '{remainder}' fails at {sequence.CurrentLocation}");
    }

    public Result<List<ArgumentsOrString>, ScriptsError> ParseScript(IReadOnlyList<string> lines, IArgumentCollection args)
    {
        var results = lines
            .Where(l => !string.IsNullOrEmpty(l))
            .Select((l, i) => CompileLine(l, i, args))
            .Aggregate(ResultsLists.New(), SeparateSuccessesAndFailures);
        if (results.Errors.Count != 0)
            return results.Errors.Aggregate((e1, e2) => e1.Combine(e2));

        return results.Arguments.ConvertAll(l => new ArgumentsOrString(new Arguments(l, _mapper)));
    }

    private static ResultsLists SeparateSuccessesAndFailures(ResultsLists l, Result<IReadOnlyList<IArgument>, ScriptsError> r)
        => r.Match(l.Add, e =>
        {
            if (e is not EmptyLine)
                l.Add(e);
            return l;
        });

    private Result<IReadOnlyList<IArgument>, ScriptsError> CompileLine(string script, int line, IArgumentCollection args)
        => ParseScriptToCommandFormat(script, line).Bind(f => f.Format(args, line));

    private Result<CommandFormat, ScriptsError> ParseScriptToCommandFormat(string script, int line)
    {
        var input = FromString(script);
        var parseResult = _scriptParser.Parse(input);
        if (!parseResult.Success)
            return new ParseFailure(line, script, parseResult.ErrorMessage);
        if (!input.IsAtEnd)
            return new ParseIncomplete(line, script, input.GetRemainder());
        return parseResult.Value;
    }

    // Keeps track of a list of successes and failures from all parse results
    private readonly record struct ResultsLists(List<IArgumentCollection> Arguments, List<ScriptsError> Errors)
    {
        public static ResultsLists New() => new ResultsLists([], []);

        public ResultsLists Add(IReadOnlyList<IArgument> s)
        {
            var args = s.ToSyntheticArguments();
            args.Reset();
            Arguments.Add(args);
            return this;
        }

        public ResultsLists Add(ScriptsError error)
        {
            Errors.Add(error);
            return this;
        }
    }
}
