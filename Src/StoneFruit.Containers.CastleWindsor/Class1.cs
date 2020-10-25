using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;
using Castle.Windsor.Extensions.DependencyInjection.Extensions;

namespace StoneFruit.Containers.CastleWindsor
{
    //public class HandlerWindsorInstaller : IWindsorInstaller
    //{
    //    public void Install(IWindsorContainer container, IConfigurationStore store)
    //    {
    //        container.Register(
    //            AllTypes.FromThisAssembly().Pick()
    //                .WithService.DefaultInterfaces()
    //                .Configure(c => c.Lifestyle.Transient)
    //        );
    //    }
    //}

    public class WindsorServiceCollection : IServiceCollection
    {
        private readonly List<Microsoft.Extensions.DependencyInjection.ServiceDescriptor> _services;

        public WindsorServiceCollection()
        {
            _services = new List<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>();
        }

        public Microsoft.Extensions.DependencyInjection.ServiceDescriptor this[int index]
        {
            get => _services[index];
            set => _services[index] = value;
        }

        public void Clear() => _services.Clear();

        public bool Contains(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item) => _services.Contains(item);

        public void CopyTo(Microsoft.Extensions.DependencyInjection.ServiceDescriptor[] array, int arrayIndex)
            => _services.CopyTo(array, arrayIndex);

        public int Count => _services.Count;

        public int IndexOf(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item) => _services.IndexOf(item);

        public void Insert(int index, Microsoft.Extensions.DependencyInjection.ServiceDescriptor item) => _services.Insert(index, item);

        public bool IsReadOnly => false;

        public void RemoveAt(int index) => _services.RemoveAt(index);

        public bool Remove(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item) => _services.Remove(item);

        public IEnumerator<Microsoft.Extensions.DependencyInjection.ServiceDescriptor> GetEnumerator() => _services.GetEnumerator();

        void ICollection<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>.Add(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
            => _services.Add(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class WindsorContainerExtensions
    {
        public static WindsorContainer SetupEngine<TEnvironment>(this IWindsorContainer container, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            var serviceCollection = new WindsorServiceCollection();
            EngineBuilder.SetupEngineRegistrations(serviceCollection);
            foreach (var serviceDescriptor in serviceCollection)
                container.Register(serviceDescriptor.CreateWindsorRegistration());
            


        }
    }
}
