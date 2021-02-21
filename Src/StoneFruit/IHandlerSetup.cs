using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Handlers;
using StoneFruit.Utility;

namespace StoneFruit
{
    public interface IHandlerSetup
    {
        /// <summary>
        /// Set the Verb Extractor to use to get verbs from classes and methods where verbs are
        /// not explicitly supplied
        /// </summary>
        /// <param name="verbExtractor"></param>
        /// <returns></returns>
        IHandlerSetup UseVerbExtractor(IVerbExtractor verbExtractor);

        IHandlerSetup UseMethodInvoker(IHandlerMethodInvoker invoker);

        /// <summary>
        /// Add a new handler source to the list of sources. Sources are stored in order, sources
        /// added first will be searched first for a matching handler.
        /// </summary>
        /// <param name="getSource"></param>
        /// <returns></returns>
        IHandlerSetup AddSource(Func<HandlerSourceBuildContext, IHandlerSource> getSource);

        /// <summary>
        /// Add a function delegate as a handler
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handle"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup Add(Verb verb, Action<IArguments, CommandDispatcher> handle, string description = "", string usage = "", string group = "");

        /// <summary>
        /// Add a pre-existing handler instance with the given verb
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handler"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "");

        /// <summary>
        /// Add a function delegate as a handler for asynchronous invokation
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handleAsync"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup AddAsync(Verb verb, Func<IArguments, CommandDispatcher, Task> handleAsync, string description = "", string usage = "", string group = "");

        /// <summary>
        /// Add a script with a verb and zero or more commands to execute
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="lines"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "");
    }

    public struct HandlerSourceBuildContext
    {
        public HandlerSourceBuildContext(IVerbExtractor verbExtractor, IHandlerMethodInvoker invoker)
        {
            VerbExtractor = verbExtractor;
            MethodInvoker = invoker;
        }

        public IVerbExtractor VerbExtractor { get; }
        public IHandlerMethodInvoker MethodInvoker { get; }
    }

    public static class HandlerSetupExtensions
    {
        /// <summary>
        /// Add a handler source where handlers can be looked up
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IHandlerSetup AddSource(this IHandlerSetup handlers, IHandlerSource source)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            Assert.ArgumentNotNull(source, nameof(source));
            return handlers.AddSource(_ => source);
        }

        /// <summary>
        /// Use the public methods of an instance object as handlers
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IHandlerSetup UsePublicMethodsAsHandlers(this IHandlerSetup handlers, object instance, Func<string, string>? getDescription = null, Func<string, string>? getUsage = null, Func<string, string>? getGroup = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            return handlers.AddSource(ctx => new InstanceMethodHandlerSource(instance, getDescription, getUsage, getGroup, ctx.MethodInvoker, ctx.VerbExtractor));
        }

        /// <summary>
        /// Use the specified list of handler types, with the default instance resolver and
        /// default verb extractor
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="commandTypes"></param>
        /// <returns></returns>
        public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, params Type[] commandTypes)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            Assert.ArgumentNotNull(commandTypes, nameof(commandTypes));
            return handlers.UseHandlerTypes(commandTypes, null);
        }

        /// <summary>
        /// Specify an explicit list of handler types to register with the Engine. Notice that these types
        /// may not be constructed using your DI container of choice. If you are using a DI container, you
        /// should try to register types with the container instead.
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="resolver"></param>
        /// <param name="verbExtractor"></param>
        /// <returns></returns>
        public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, IEnumerable<Type> commandTypes, TypeInstanceResolver? resolver = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            Assert.ArgumentNotNull(commandTypes, nameof(commandTypes));
            return handlers.AddSource(ctx => new TypeListConstructSource(commandTypes, resolver, ctx.VerbExtractor));
        }
    }
}
