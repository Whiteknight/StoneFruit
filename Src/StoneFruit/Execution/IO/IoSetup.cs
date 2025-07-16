using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace StoneFruit.Execution.IO;

public class IoSetup : IIoSetup
{
    private readonly List<IOutput> _outputs;
    private bool _useConsole;
    private IInput? _input;
    private ICommandLine? _commandLine;

    public IoSetup()
    {
        _useConsole = true;
        _outputs = [];
    }

    public IIoSetup DoNotUseConsole()
    {
        _useConsole = false;
        return this;
    }

    public IIoSetup Add(IOutput output)
    {
        if (output != null)
            _outputs.Add(output);
        return this;
    }

    public IIoSetup Use(IInput input)
    {
        _input = input;
        return this;
    }

    public IIoSetup SetCommandLine(ICommandLine commandLine)
    {
        _commandLine = commandLine;
        return this;
    }

    public IIoSetup SetCommandLine(string commandLine)
        => SetCommandLine(new StringCommandLine(commandLine));

    public void BuildUp(IServiceCollection services)
    {
        services.TryAddSingleton(_commandLine ?? new EnvironmentCommandLine());
        services.TryAddSingleton(BuildOutput());
        services.TryAddSingleton(BuildInput());
    }

    private IInput BuildInput()
    {
        return _input ?? (_useConsole ? new ConsoleIO() : new NullIO());
    }

    private IOutput BuildOutput()
    {
        if (!_useConsole)
        {
            if (_outputs.Count == 0)
                return new NullIO();
            if (_outputs.Count == 1)
                return _outputs[0];
            return new CombinedOutput(_outputs);
        }

        return _outputs.Count == 0
            ? new ConsoleIO()
            : new CombinedOutput(new ConsoleIO(), _outputs);
    }
}
