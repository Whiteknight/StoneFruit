using System;
using Unity;

namespace StoneFruit.Containers.Unity
{
    public class UnityServiceProvider : IServiceProvider
    {
        private readonly IUnityContainer _container;

        public UnityServiceProvider(IUnityContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType) => _container.Resolve(serviceType);
    }
}