using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Execution.IO;

public class IoSetup : IIoSetup
{
    private readonly IServiceCollection _services;

    public IoSetup(IServiceCollection services)
    {
        _services = services;
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

    public static void BuildUp(IServiceCollection services)
    {
        services.TryAddSingleton<IOutput, ConsoleIO>();
        services.TryAddSingleton<IInput, ConsoleIO>();
        services.TryAddSingleton<ICommandLine, EnvironmentCommandLine>();
        services.TryAddSingleton(_ => DefaultTemplateFormat.Parser.Create());
        services.TryAddSingleton(typeof(ILogger<>), typeof(OutputLogger<>));
    }
}
