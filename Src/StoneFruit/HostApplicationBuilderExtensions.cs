using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StoneFruit;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder SetupStoneFruit(this IHostApplicationBuilder app, Action<IEngineBuilder> buildEngine)
    {
        var engine = EngineBuilder.Build(app.Services, buildEngine);
        app.Services.AddSingleton<Engine>(engine);
        app.Services.AddHostedService<StoneFruitHostedService>();
        return app;
    }
}

public class StoneFruitHostedService : IHostedService, IHostedLifecycleService
{
    private readonly Engine _engine;

    public StoneFruitHostedService(Engine engine)
    {
        _engine = engine;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // TODO: Take cancellationtoken
        Environment.ExitCode = await _engine.RunWithCommandLineArgumentsAsync();
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // TODO: Need to be able to signal exit AND wait for the engine to wrap up everything.
        _engine.State.SignalExit();
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
