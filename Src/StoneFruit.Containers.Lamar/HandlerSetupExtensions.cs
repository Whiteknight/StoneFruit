using Lamar;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Lamar
{
    public static class HandlerSetupExtensions
    {
        /// <summary>
        /// Use the Lamar DI container to find and construct handler objects. 
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="container">A pre-existing container to use</param>
        /// <param name="verbExtractor"></param>
        /// <returns></returns>
        public static IHandlerSetup UseLamarHandlerSource<TEnvironment>(this IHandlerSetup handlers, IContainer container = null, ITypeVerbExtractor verbExtractor = null)
            where TEnvironment : class
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            //var source = new LamarHandlerSource<TEnvironment>(container, verbExtractor);
            //return handlers.AddSource(source);
            return handlers;
        }
    }
}