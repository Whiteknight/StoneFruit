using System;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Setup Engine registrations in the StructureMap container. This method will scan
        /// assemblies in your solution for handler types. Any handler types which cannot be found
        /// during scanning should be manually registered with the container prior to calling this
        /// method. This variant uses an environment object.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="container"></param>
        /// <param name="build"></param>
        public static IContainer SetupEngine<TEnvironment>(this IContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var services = new DefaultServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build, () => ScanForHandlers(container));
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);

            services.AddSingleton<IHandlerSource>(provider =>
            {
                var verbExtractor = provider.GetService<IVerbExtractor>();
                return new StructureMapHandlerSource(provider, verbExtractor);
            });
            container.Populate(services);
            return container;
        }

        /// <summary>
        /// Setup Engine registrations in the StructureMap container. This method will scan
        /// assemblies in your solution for Handler types. Any Handler types which cannot be found
        /// during scanning should be manually registered with the container prior to calling this
        /// method. This variant does not use an environment object.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static IContainer SetupEngine(this IContainer container, Action<IEngineBuilder> build)
        {
            var services = new DefaultServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build, () => ScanForHandlers(container));

            services.AddSingleton<IHandlerSource>(provider =>
            {
                var verbExtractor = provider.GetService<IVerbExtractor>();
                return new StructureMapHandlerSource(provider, verbExtractor);
            });
            container.Populate(services);
            return container;
        }

        private static void ScanForHandlers(IContainer container)
        {
            container.Configure(c =>
            {
                c.ScanForCommandVerbs();
            });
        }
    }
}
