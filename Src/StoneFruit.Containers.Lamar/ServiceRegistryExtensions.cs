using System;
using System.Collections.Generic;
using System.Text;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Containers.Lamar
{
    public static class ServiceRegistryExtensions
    {
        public static ServiceRegistry SetupEngine<TEnvironment>(this ServiceRegistry services, Action<EngineBuilder> build)
            where TEnvironment : class
        {
            var builder = new EngineBuilder();
            build?.Invoke(builder);
            builder.BuildUp(services);
            services.Scan(s => s.ScanForHandlers());
            services.AddSingleton(provider => new EngineAccessor());
            services.AddSingleton(provider =>
            {
                var accessor = provider.GetService<EngineAccessor>();
                var handlers = provider.GetService<HandlerSourceCollection>();
                var environments = provider.GetService<IEnvironmentCollection>();
                var parser = provider.GetService<ICommandParser>();
                var output = provider.GetService<IOutput>();
                var engineCatalog = provider.GetService<EngineEventCatalog>();
                var engineSettings = provider.GetService<EngineSettings>();
                var e = new Engine(handlers, environments, parser, output, engineCatalog, engineSettings);
                accessor.SetEngine(e);
                return e;
            });
            services.AddTransient(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentState());
            services.AddTransient(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentDispatcher());
            services.AddSingleton<IHandlerSource>(provider => new LamarHandlerSource<TEnvironment>(provider, TypeVerbExtractor.DefaultInstance));
            services.AddSingleton<IHandlerSource>(HandlerSource.GetBuiltinHandlerSource());
            services.Injectable<Command>();
            services.Injectable<IArguments>();
            if (typeof(TEnvironment) != typeof(object))
                services.Injectable<TEnvironment>();
            return services;
        }
    }
}
