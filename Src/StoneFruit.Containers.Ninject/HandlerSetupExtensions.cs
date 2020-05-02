using Ninject;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Ninject
{
    public static class HandlerSetupExtensions
    {
        // TODO: Provide a custom type resolver
        public static IHandlerSetup UseNinjectHandlerSource(this IHandlerSetup handlers, IKernel kernel = null, ITypeVerbExtractor verbExtractor = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));

            var source = new NinjectHandlerSource(kernel, verbExtractor);
            return handlers.AddSource(source);
        }
    }
}
