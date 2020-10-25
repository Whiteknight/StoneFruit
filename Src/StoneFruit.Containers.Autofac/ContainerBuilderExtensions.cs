using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder SetupEngine<TEnvironment>(this ContainerBuilder containerBuilder, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var serviceCollection = new AutofacServiceCollection();
            EngineBuilder.SetupEngineRegistrations(serviceCollection, build);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(serviceCollection);
            serviceCollection.AddSingleton<IHandlerSource>(provider =>
            {
                var verbExtractor = provider.GetService<IVerbExtractor>();
                return new AutofacHandlerSource(provider, verbExtractor);
            });
            containerBuilder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).Where(t => typeof(IHandlerBase).IsAssignableFrom(t)).AsSelf();
            containerBuilder.Populate(serviceCollection);
            return containerBuilder;
        }
    }
}
