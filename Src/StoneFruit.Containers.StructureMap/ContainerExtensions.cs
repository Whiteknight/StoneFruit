using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace StoneFruit.Containers.StructureMap
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Setup Engine registrations in the StructureMap container. This method will scan assemblies in
        /// your solution for handler types. Any handler types which cannot be found during scanning should
        /// be manually registered with the container prior to calling this method.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="container"></param>
        /// <param name="build"></param>
        public static void SetupEngine<TEnvironment>(this IContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var services = new StructureMapServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);

            services.AddSingleton<IHandlerSource>(provider => new StructureMapHandlerSource(provider, TypeVerbExtractor.DefaultInstance));
            container.Configure(c =>
            {
                c.ScanForCommandVerbs();
            });
            container.Populate(services);
        }
    }
}
