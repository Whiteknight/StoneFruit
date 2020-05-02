using Ninject;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Ninject
{
    public static class HandlerSetupExtensions
    {
        /// <summary>
        /// Tell the HandlerSetup to use Ninject to scan for and resolve handler types in your solution.
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="kernel"></param>
        /// <param name="verbExtractor"></param>
        /// <returns></returns>
        public static IHandlerSetup UseNinjectHandlerSource(this IHandlerSetup handlers, IKernel kernel = null, ITypeVerbExtractor verbExtractor = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));

            var source = new NinjectHandlerSource(kernel, verbExtractor);
            return handlers.AddSource(source);
        }
    }
}
