using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Containers.Microsoft
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Setup the engine registration in the DI container. 
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <param name="getProvider"></param>
        /// <returns></returns>
        public static IServiceCollection SetupEngine<TEnvironment>(this IServiceCollection services, Action<IEngineBuilder> build, Func<IServiceProvider> getProvider) where TEnvironment : class
        {
            // Setup a custom resolver for manually-specified types
            var resolver = new MicrosoftTypeInstanceResolver(getProvider);
            var handlersBuilder = new HandlerSetup(resolver.Resolve);

            // Scan for handler classes in all assemblies, and setup a source to pull those types out of the
            // provider
            services.Scan(scanner => scanner
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<IHandlerBase>())
                .AsSelf()
                .WithTransientLifetime()
            );
            var registeredHandlerSource = new MicrosoftRegisteredHandlerSource(services, getProvider, TypeVerbExtractor.DefaultInstance);
            handlersBuilder.AddSource(registeredHandlerSource);

            // Setup all engine registrations
            var builder = new EngineBuilder(handlers: handlersBuilder);
            EngineBuilder.SetupEngineRegistrations(builder, services, build);
            
            // Setup the environment
            services.AddTransient<TEnvironment>(provider => provider.GetService<IEnvironmentCollection>().Current as TEnvironment);

            return services;
        }
    }
}
