using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.Lifetime;
using Unity.RegistrationByConvention;

namespace StoneFruit.Containers.Unity
{
    /// <summary>
    /// Extension methods for the IUnityContainer to setup the StoneFruit Engine and dependencies.
    /// </summary>
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// Setup registrations for the Engine. Scan assemblies in the application to find Handler
        /// types to register with the container. This variant uses an environment object.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="container"></param>
        /// <param name="build"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Setup registrations for the Engine. Scan assemblies in the application to find Handler
        /// types to register with the container. This variant does not use an environment object.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static IUnityContainer SetupEngine(this IUnityContainer container, Action<IEngineBuilder> build)
        {
            var handlerTypes = AllClasses.FromAssembliesInBasePath().Where(t => !t.IsAbstract && typeof(IHandlerBase).IsAssignableFrom(t)).ToList();
            container.RegisterTypes(
                handlerTypes,
                WithMappings.None,
                WithName.Default,
                WithLifetime.Transient
            );
            return SetupEngineScannerless(container, build);
        }

        /// <summary>
        /// Setup registrations for the Engine. Does not scan assemblies for Handler types
        /// automatically. You must register Handler types with the container separately. This
        /// variant uses an environment object.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="container"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static IUnityContainer SetupEngineScannerless<TEnvironment>(this IUnityContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var services = new DefaultServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);
            foreach (var descriptor in services)
                AddRegistration(container, descriptor);
            container.RegisterFactory<IHandlerSource>(c => new UnityHandlerSource(c, c.Resolve<IVerbExtractor>()));
            return container;
        }

        /// <summary>
        /// Setup registrations for the Engine. Does not scan assemblies for Handler types
        /// automatically. You must register Handler types with the container separately. This
        /// variant does not use an environment object.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static IUnityContainer SetupEngineScannerless(this IUnityContainer container, Action<IEngineBuilder> build)
        {
            var services = new DefaultServiceCollection();
            EngineBuilder.SetupEngineRegistrations(services, build);
            foreach (var descriptor in services)
                AddRegistration(container, descriptor);
            container.RegisterFactory<IHandlerSource>(c => new UnityHandlerSource(c, c.Resolve<IVerbExtractor>()));
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

        private static LifetimeManager GetLifetime(this ServiceDescriptor serviceDescriptor)
            => serviceDescriptor.Lifetime switch
            {
                ServiceLifetime.Scoped => new HierarchicalLifetimeManager(),
                ServiceLifetime.Singleton => new SingletonLifetimeManager(),
                ServiceLifetime.Transient => new TransientLifetimeManager(),
                _ => throw new InvalidOperationException("Unsupported lifetime"),
            };
    }
}
