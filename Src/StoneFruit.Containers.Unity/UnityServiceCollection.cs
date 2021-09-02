using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Unity
{
    public class UnityServiceCollection : IServiceCollection
    {
        private readonly List<ServiceDescriptor> _services;

        public UnityServiceCollection()
        {
            _services = new List<ServiceDescriptor>();
        }

        public ServiceDescriptor this[int index]
        {
            get => _services[index];
            set => _services[index] = value;
        }

        public void Clear() => _services.Clear();

        public bool Contains(ServiceDescriptor item) => _services.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            => _services.CopyTo(array, arrayIndex);

        public int Count => _services.Count;

        public int IndexOf(ServiceDescriptor item) => _services.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) => _services.Insert(index, item);

        public bool IsReadOnly => false;

        public void RemoveAt(int index) => _services.RemoveAt(index);

        public bool Remove(ServiceDescriptor item) => _services.Remove(item);

        public IEnumerator<ServiceDescriptor> GetEnumerator() => _services.GetEnumerator();

        void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item)
            => _services.Add(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}