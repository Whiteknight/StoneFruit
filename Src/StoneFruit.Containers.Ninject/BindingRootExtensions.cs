using Ninject.Syntax;

namespace StoneFruit.Containers.Ninject
{
    public static class BindingRootExtensions
    {
        /// <summary>
        /// Convenience method to bind a singleton instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="instance"></param>
        public static void BindInstance<T>(this IBindingRoot container, T instance)
        {
            container.Bind<T>().ToConstant(instance).InSingletonScope();
        }
    }
}