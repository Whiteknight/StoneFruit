using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Execution.IO;

public class IoSetup : IIoSetup
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<LogLevel, Brush> _logColors;
    private LogLevel _logLevel = LogLevel.Information;

    public IoSetup(IServiceCollection services)
    {
        _services = services;
        _logColors = LoggingConfiguration.GetColorsLookup();
    }

    public IIoSetup UseOutput<T>()
        where T : class, IOutput
    {
        _services.AddSingleton<IOutput, T>();
        return this;
    }

    public IIoSetup UseOutput(IOutput output)
    {
        _services.AddSingleton(output);
        return this;
    }

    public IIoSetup UseInput(IInput input)
    {
        _services.AddSingleton(input);
        return this;
    }

    public IIoSetup UseInput<T>()
        where T : class, IInput
    {
        _services.AddSingleton<IInput, T>();
        return this;
    }

    public IIoSetup SetCommandLine(ICommandLine commandLine)
    {
        _services.AddSingleton(commandLine);
        return this;
    }

    public IIoSetup SetCommandLine(string commandLine)
        => SetCommandLine(new StringCommandLine(commandLine));

    public IIoSetup UseTemplateParser(ITemplateParser parser)
    {
        _services.AddSingleton(parser);
        return this;
    }

    public IIoSetup UseLogLevel(LogLevel logLevel)
    {
        _logLevel = logLevel;
        return this;
    }

    public IIoSetup SetLogColor(LogLevel logLevel, Brush brush)
    {
        _logColors[logLevel] = brush;
        return this;
    }

    public IIoSetup UseObjectWriter<T>()
        where T : class, IObjectOutputWriter
    {
        _services.TryAddSingleton<IObjectOutputWriter, T>();
        return this;
    }

    public void BuildUp(IServiceCollection services)
    {
        // Fill in some default values in case they haven't been specified elsewhere.
        services.TryAddSingleton<IOutput, ConsoleIO>();
        services.TryAddSingleton<IInput, ConsoleIO>();
        services.TryAddSingleton<ICommandLine, EnvironmentCommandLine>();
        services.TryAddSingleton(_ => DefaultTemplateFormat.Parser.Create());
        services.TryAddSingleton(typeof(ILogger<>), typeof(OutputLogger<>));
        services.TryAddSingleton(new LoggingConfiguration(_logLevel, _logColors));
        services.TryAddSingleton<ILoggerFactory, OutputLoggerFactory>();
        services.TryAddSingleton<IObjectOutputWriter, NullObjectOutputWriter>();
    }
}
