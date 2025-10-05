using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using ParserObjects;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Scripts;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Sets up the parsers.
/// </summary>
public class ArgumentsSetup : IArgumentsSetup
{
    private readonly Dictionary<Type, IValueTypeParser> _typeParsers;
    private IParser<char, IArgumentToken>? _argParser;
    private IParser<char, CommandFormat>? _scriptParser;

    public ArgumentsSetup()
    {
        _typeParsers = [];
    }

    public void BuildUp(IServiceCollection services)
    {
        services.AddSingleton(_argParser ?? SimplifiedArgumentGrammar.GetParser());
        services.AddSingleton(_scriptParser ?? ScriptFormatGrammar.GetParser());
        services.AddSingleton<ICommandParser, CommandParser>();

        MaybeUseTypeParser(a => a.AsString());
        MaybeUseTypeParser(a => Guid.Parse(a.AsString()));
        MaybeUseTypeParser(a => new FileInfo(a.AsString()));
        MaybeUseTypeParser(a => new DirectoryInfo(a.AsString()));
        foreach (var typeParser in _typeParsers)
            services.AddSingleton(typeParser.Value);

        services.AddSingleton<ArgumentValueMapper>();
    }

    public IArgumentsSetup UseArgumentParser(IParser<char, IArgumentToken> argParser)
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

    public IArgumentsSetup UseScriptParser(IParser<char, CommandFormat> scriptParser)
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

    public IArgumentsSetup UseTypeParser<T>(IValueTypeParser<T> typeParser)
    {
        _typeParsers[typeof(T)] = NotNull(typeParser);
        return this;
    }

    private void MaybeUseTypeParser<T>(Func<IValuedArgument, T> parser)
    {
        _typeParsers.TryAdd(typeof(T), new DelegateValueTypeParser<T>(parser));
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
