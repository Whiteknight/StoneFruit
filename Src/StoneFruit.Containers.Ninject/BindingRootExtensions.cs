using Ninject.Syntax;
using Ninject.Extensions.Conventions;

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

        /// <summary>
        /// Convenience method to scan assemblies in the current directory for handlers
        /// </summary>
        /// <param name="kernel"></param>
        public static void ScanForHandlers(this IBindingRoot kernel)
        {
            kernel.Bind(x =>
            {
                x.FromAssemblyContaining<IHandlerBase>()
                    .SelectAllTypes()
                    .InheritedFrom<IHandlerBase>()
                    .BindAllInterfaces();
                x.FromAssembliesInPath(".")
                    .SelectAllTypes()
                    .InheritedFrom<IHandlerBase>()
                    .BindAllInterfaces();
            });
        }
    }
}