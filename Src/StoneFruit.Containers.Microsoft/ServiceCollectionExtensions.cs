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
        /// This variant registers an environment object.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <param name="getProvider"></param>
        /// <returns></returns>
        public static IServiceCollection SetupEngine<TEnvironment>(this IServiceCollection services, Action<IEngineBuilder> build, Func<IServiceProvider> getProvider)
            where TEnvironment : class
        {
            // Setup a custom resolver for manually-specified types, and use that in the
            // buildup
            var handlersBuilder = new HandlerSetup(() => ScanForHandlers(services));
            var builder = new EngineBuilder(handlers: handlersBuilder);
            build?.Invoke(builder);
            EngineBuilder.SetupEngineRegistrations(services);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);

            // Setup a Handler Source to resolve handler instances from the provider
            handlersBuilder.AddSource(ctx => new MicrosoftRegisteredHandlerSource(services, getProvider, ctx.VerbExtractor));

            // Build up the final Engine registrations. This involves resolving some
            // dependencies which were setup previously
            builder.BuildUp(services);

            return services;
        }

        /// <summary>
        /// Setup the engine registrations for the DI container. This method will automatically
        /// scan the assemblies in your solution for handler types. Any handler types which are not
        /// found during the scan should be manually registered in the ServiceCollection before
        /// calling this method. This variant does not register an environment object.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <param name="getProvider"></param>
        /// <returns></returns>
        public static IServiceCollection SetupEngine(this IServiceCollection services, Action<IEngineBuilder> build, Func<IServiceProvider> getProvider)
        {
            // Setup a custom resolver for manually-specified types, and use that in the
            // buildup
            var handlersBuilder = new HandlerSetup(() => ScanForHandlers(services));
            var builder = new EngineBuilder(handlers: handlersBuilder);
            build?.Invoke(builder);
            EngineBuilder.SetupEngineRegistrations(services);

            // Setup a Handler Source to resolve handler instances from the provider
            handlersBuilder.AddSource(ctx => new MicrosoftRegisteredHandlerSource(services, getProvider, ctx.VerbExtractor));

            // Build up the final Engine registrations. This involves resolving some
            // dependencies which were setup previously
            builder.BuildUp(services);

            return services;
        }

        private static void ScanForHandlers(IServiceCollection services)
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
        }
    }
}
