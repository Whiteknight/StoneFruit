using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Autofac
{
    /// <summary>
    /// Extension methods for the ContainerBuilder to setup the Engine and related dependencies.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Setup Engine registrations in the container. Scan assemblies in the application for
        /// Handlers and register them with the container. This variant uses an environment object.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="containerBuilder"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static ContainerBuilder SetupEngine<TEnvironment>(this ContainerBuilder containerBuilder, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var serviceCollection = new DefaultServiceCollection();
            EngineBuilder.SetupEngineRegistrations(serviceCollection, build, () => ScanForHandlers(containerBuilder));
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(serviceCollection);
            serviceCollection.AddSingleton<IHandlerSource>(provider =>
            {
                var verbExtractor = provider.GetService<IVerbExtractor>();
                return new AutofacHandlerSource(provider, verbExtractor);
            });
            containerBuilder.Populate(serviceCollection);
            return containerBuilder;
        }

        /// <summary>
        /// Setup Engine registrations in the container. Scan assemblies in the application for
        /// Handlers and register them with the container. This variant does not use an environment
        /// object.
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static ContainerBuilder SetupEngine(this ContainerBuilder containerBuilder, Action<IEngineBuilder> build)
        {
            var serviceCollection = new DefaultServiceCollection();
            EngineBuilder.SetupEngineRegistrations(serviceCollection, build, () => ScanForHandlers(containerBuilder));
            serviceCollection.AddSingleton<IHandlerSource>(provider =>
            {
                var verbExtractor = provider.GetService<IVerbExtractor>();
                return new AutofacHandlerSource(provider, verbExtractor);
            });
            containerBuilder.Populate(serviceCollection);
            return containerBuilder;
        }

        private static void ScanForHandlers(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => typeof(IHandlerBase).IsAssignableFrom(t))
                .AsSelf();
        }
    }
}
