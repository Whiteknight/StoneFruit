using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StoneFruit
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseStoneFruit(this IHostBuilder hostBuilder, Action<IEngineBuilder> build)
        {
            return hostBuilder.ConfigureServices((_, services) =>
            {
                services.AddHostedService<StoneFruitHostedService>();
                EngineBuilder.Build(build, services);
            });
        }
    }

    public class StoneFruitHostedService : IHostedService
    {
        private readonly ILogger<StoneFruitHostedService> _logger;
        private readonly Engine _engine;
        private readonly IHostApplicationLifetime _lifetime;

        public StoneFruitHostedService(ILogger<StoneFruitHostedService> logger, Engine engine, IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _engine = engine;
            _lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _lifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // TODO: _engine methods probably want to be async
                        Environment.ExitCode = _engine.RunWithCommandLineArguments();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception from StoneFruit engine");
                    }
                    finally
                    {
                        _lifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
