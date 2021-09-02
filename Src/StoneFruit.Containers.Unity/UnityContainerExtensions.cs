using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace StoneFruit.Containers.Unity
{
    public static class UnityContainerExtensions
    {
        public static IUnityContainer SetupEngine<TEnvironment>(this IUnityContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var handlerTypes = AllClasses.FromAssembliesInBasePath().Where(t => !t.IsAbstract && typeof(IHandlerBase).IsAssignableFrom(t)).ToList();
            container.RegisterTypes(
                handlerTypes,
                WithMappings.None,
                WithName.Default,
                WithLifetime.Transient
            );
            return SetupEngineScannerless<TEnvironment>(container, build);
        }

        public static IUnityContainer SetupEngine(this IUnityContainer container, Action<IEngineBuilder> build)
        {
            container.RegisterTypes(
                AllClasses.FromAssembliesInBasePath().Where(t => t.IsPublic && !t.IsAbstract && typeof(IHandlerBase).IsAssignableFrom(t)),
                WithMappings.None,
                WithName.Default,
                WithLifetime.Transient
            );
            return SetupEngineScannerless(container, build);
        }

        public static IUnityContainer SetupEngineScannerless<TEnvironment>(this IUnityContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var services = new UnityServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);
            foreach (var descriptor in services)
                AddRegistration(container, descriptor);
            container.RegisterFactory<IHandlerSource>(c => new UnityHandlerSource(c, TypeVerbExtractor.DefaultInstance));
            return container;
        }

        public static IUnityContainer SetupEngineScannerless(this IUnityContainer container, Action<IEngineBuilder> build)
        {
            var services = new UnityServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);
            foreach (var descriptor in services)
                AddRegistration(container, descriptor);
            container.RegisterFactory<IHandlerSource>(c => new UnityHandlerSource(c, TypeVerbExtractor.DefaultInstance));
            return container;
        }

        private static void AddRegistration(IUnityContainer container, ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor.ImplementationType != null)
            {
                var name = serviceDescriptor.ServiceType.IsGenericTypeDefinition ? UnityContainer.All : null;
                container.RegisterType(
                    serviceDescriptor.ServiceType,
                    serviceDescriptor.ImplementationType,
                    name,
                    (ITypeLifetimeManager)serviceDescriptor.GetLifetime()
                );
                return;
            }

            if (serviceDescriptor.ImplementationFactory != null)
            {
                container.RegisterFactory(
                    serviceDescriptor.ServiceType,
                    null,
                    scope => serviceDescriptor.ImplementationFactory(new UnityServiceProvider(scope)),
                    (IFactoryLifetimeManager)serviceDescriptor.GetLifetime()
                );
                return;
            }

            if (serviceDescriptor.ImplementationInstance != null)
            {
                container.RegisterInstance(
                    serviceDescriptor.ServiceType,
                    null,
                    serviceDescriptor.ImplementationInstance,
                    (IInstanceLifetimeManager)serviceDescriptor.GetLifetime()
                );
                return;
            }

            throw new InvalidOperationException("Unsupported registration type");
        }

        internal static LifetimeManager GetLifetime(this ServiceDescriptor serviceDescriptor)
        {
            switch (serviceDescriptor.Lifetime)
            {
                case ServiceLifetime.Scoped:
                    return new HierarchicalLifetimeManager();

                case ServiceLifetime.Singleton:
                    return new SingletonLifetimeManager();

                case ServiceLifetime.Transient:
                    return new TransientLifetimeManager();

                default:
                    throw new InvalidOperationException("Unsupported lifetime");
            }
        }
    }
}