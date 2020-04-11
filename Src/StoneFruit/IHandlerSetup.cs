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
        // TODO V2: Some kind of mechanism for a verb to invoke an external application

        /// <summary>
        /// Add a handler source where handlers can be looked up
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IHandlerSetup AddSource(IHandlerSource source);

        /// <summary>
        /// Add a function delegate as a handler
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handle"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup Add(string verb, Action<Command, CommandDispatcher> handle, string description = null, string usage = null);

        /// <summary>
        /// Add a function delegate as a handler for asynchronous invokation
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handleAsync"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup AddAsync(string verb, Func<Command, CommandDispatcher, Task> handleAsync, string description = null, string usage = null);

        /// <summary>
        /// Add a pre-existing handler instance with the given verb
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handler"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup Add(string verb, IHandlerBase handler, string description = null, string usage = null);

        /// <summary>
        /// Add a script with a verb and zero or more commands to execute
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="lines"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup AddScript(string verb, IEnumerable<string> lines, string description = null, string usage = null);

        /// <summary>
        /// Adds one or more aliases for a verb
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="aliases"></param>
        /// <returns></returns>
        IHandlerSetup AddAlias(string verb, params string[] aliases);
    }

    public static class HandlerSetupExtensions
    {
        /// <summary>
        /// Use the public methods of an instance object as handlers
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IHandlerSetup UsePublicMethodsAsHandlers(this IHandlerSetup handlers, object instance)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            var source = new InstanceMethodHandlerSource(instance, null, null);
            return handlers.AddSource(source);
        }

        /// <summary>
        /// Specify a literal list of handler types to use
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="commandTypes"></param>
        /// <param name="verbExtractor"></param>
        /// <returns></returns>
        public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, IEnumerable<Type> commandTypes, ITypeVerbExtractor verbExtractor = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            Assert.ArgumentNotNull(commandTypes, nameof(commandTypes));
            var source = new TypeListConstructSource(commandTypes, verbExtractor);
            return handlers.AddSource(source);
        }

        /// <summary>
        /// Specify a literal list of handler types to use
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="commandTypes"></param>
        /// <returns></returns>
        public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, params Type[] commandTypes)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            Assert.ArgumentNotNull(commandTypes, nameof(commandTypes));
            return handlers.AddSource(new TypeListConstructSource(commandTypes, null));
        }
    }
}