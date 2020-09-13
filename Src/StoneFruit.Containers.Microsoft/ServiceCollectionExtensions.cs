using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Containers.Microsoft
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Setup the engine registrations in the DI container. This method will automatically scan the
        /// assemblies in your solution for handler types. Any handler types which are not found during the
        /// scan should be manually registered in the ServiceCollection before calling this method.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <param name="getProvider"></param>
        /// <returns></returns>
        public static IServiceCollection SetupEngine<TEnvironment>(this IServiceCollection services, Action<IEngineBuilder> build, Func<IServiceProvider> getProvider)
            where TEnvironment : class
        {
            // Scan for handler classes in all assemblies, and setup a source to pull those types out of the
            // provider
            services.Scan(scanner => scanner
                .FromApplicationDependencies()
                .AddClasses(classes => classes.Where(t =>
                {
                    if (!typeof(IHandlerBase).IsAssignableFrom(t))
                        return false;
                    if (!t.IsPublic)
                        return false;
                    return true;
                }))
                .AsSelf()
                .WithTransientLifetime()
            );

            return SetupEngineScannerless<TEnvironment>(services, build, getProvider);
        }

        /// <summary>
        /// Setup the Engine registrations in the DI container. This method will not scan
        /// for handler classes in your solution, you must have them registered already
        /// before calling this method.  
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <param name="getProvider"></param>
        /// <returns></returns>
        public static IServiceCollection SetupEngineScannerless<TEnvironment>(this IServiceCollection services, Action<IEngineBuilder> build, Func<IServiceProvider> getProvider)
            where TEnvironment : class
        {
            // Setup a custom resolver for manually-specified types, and use that in the
            // buildup
            var handlersBuilder = new HandlerSetup();
            var builder = new EngineBuilder(handlers: handlersBuilder);
            build?.Invoke(builder);
            EngineBuilder.SetupEngineRegistrations(services);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);

            // Setup a Handler Source to resolve handler instances from the provider
            var registeredHandlerSource = new MicrosoftRegisteredHandlerSource(services, getProvider, TypeVerbExtractor.DefaultInstance);
            handlersBuilder.AddSource(registeredHandlerSource);

            // Build up the final Engine registrations. This involves resolving some
            // dependencies which were setup previously
            builder.BuildUp(services);

            return services;
        }
    }
}
