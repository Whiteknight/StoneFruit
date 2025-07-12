using Microsoft.Extensions.DependencyInjection;
using ParserObjects;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Sets up the parsers.
/// </summary>
public class ParserSetup : IParserSetup
{
    private IParser<char, ArgumentToken>? _argParser;
    private IParser<char, CommandFormat>? _scriptParser;

    public void BuildUp(IServiceCollection services)
    {
        services.AddSingleton<ICommandParser>(new CommandParser(
            _argParser ?? SimplifiedArgumentGrammar.GetParser(),
            _scriptParser ?? ScriptFormatGrammar.GetParser()));
    }

    public IParserSetup UseArgumentParser(IParser<char, ArgumentToken> argParser)
    {
        if (argParser == null)
        {
            _argParser = null;
            return this;
        }

        EnsureCanSetArgumentParser();
        _argParser = argParser;
        return this;
    }

    public IParserSetup UseScriptParser(IParser<char, CommandFormat> scriptParser)
    {
        if (scriptParser == null)
        {
            _scriptParser = null;
            return this;
        }

        EnsureCanSetScriptParser();
        _scriptParser = scriptParser;
        return this;
    }

    private void EnsureCanSetArgumentParser()
    {
        if (_argParser != null)
            throw new EngineBuildException("Argument parser is already set for this builder. You cannot set a second argument parser.");
    }

    private void EnsureCanSetScriptParser()
    {
        if (_scriptParser != null)
            throw new EngineBuildException("Script parser is already set for this builder. You cannot set a second script parser.");
    }
}
