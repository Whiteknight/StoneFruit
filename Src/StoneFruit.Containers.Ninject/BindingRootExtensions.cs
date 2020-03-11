using Ninject.Syntax;

namespace StoneFruit.Containers.Ninject
{
    public static class BindingRootExtensions
    {
        public static void BindInstance<T>(this IBindingRoot container, T instance)
        {
            container.Bind<T>().ToConstant(instance).InSingletonScope();
        }
    }
}