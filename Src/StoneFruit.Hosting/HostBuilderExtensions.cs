using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoneFruit.Containers.Microsoft;

namespace StoneFruit
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseStoneFruit(this IHostBuilder hostBuilder, Action<IEngineBuilder> build)
        {
            return hostBuilder.ConfigureServices((buildContext, services) =>
            {
                services.AddHostedService<StoneFruitHostedService>();
                services.SetupEngine(build, () => services.BuildServiceProvider());
            });
        }
    }

    public class StoneFruitHostedService : IHostedService
    {
        private readonly Engine _engine;

        public StoneFruitHostedService(Engine engine)
        {
            _engine = engine;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: _engine methods probably want to be async
            Environment.ExitCode = _engine.RunWithCommandLineArguments();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
