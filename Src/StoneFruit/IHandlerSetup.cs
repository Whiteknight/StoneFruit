using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Handlers;
using StoneFruit.Utility;

namespace StoneFruit
{
    public interface IHandlerSetup
    {
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
        IHandlerSetup Add(Verb verb, Action<IArguments, CommandDispatcher> handle, string description = null, string usage = null, string group = null);

        /// <summary>
        /// Add a pre-existing handler instance with the given verb
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handler"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup Add(Verb verb, IHandlerBase handler, string description = null, string usage = null, string group = null);

        /// <summary>
        /// Add a function delegate as a handler for asynchronous invokation
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handleAsync"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup AddAsync(Verb verb, Func<IArguments, CommandDispatcher, Task> handleAsync, string description = null, string usage = null, string group = null);

        /// <summary>
        /// Add a script with a verb and zero or more commands to execute
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="lines"></param>
        /// <param name="description"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = null, string usage = null, string group = null);

        ///// <summary>
        ///// Adds one or more aliases for a verb
        ///// </summary>
        ///// <param name="verb"></param>
        ///// <param name="aliases"></param>
        ///// <returns></returns>
        //IHandlerSetup AddAlias(Verb verb, params string[] aliases);

        /// <summary>
        /// Specify an explicit list of handler types to register with the Engine. Notice that these types
        /// may not be constructed using your DI container of choice. If you are using a DI container, you
        /// should try to register types with the container instead.
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="resolver"></param>
        /// <param name="verbExtractor"></param>
        /// <returns></returns>
        IHandlerSetup UseHandlerTypes(IEnumerable<Type> commandTypes, TypeInstanceResolver resolver = null, ITypeVerbExtractor verbExtractor = null);

        /// <summary>
        /// Build up registrations in the provided service collection
        /// </summary>
        /// <param name="services"></param>
        void BuildUp(IServiceCollection services);

        /// <summary>
        /// Build the IHandlers instance directly
        /// </summary>
        /// <returns></returns>
        IHandlers Build();
    }

    public static class HandlerSetupExtensions
    {
        /// <summary>
        /// Use the public methods of an instance object as handlers
        /// </summary>
        /// <param name="handlers"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IHandlerSetup UsePublicMethodsAsHandlers(this IHandlerSetup handlers, object instance, Func<string, string> getDescription = null, Func<string, string> getUsage = null, Func<string, string> getGroup = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            var source = new InstanceMethodHandlerSource(instance, getDescription, getUsage, getGroup);
            return handlers.AddSource(source);
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
            return handlers.UseHandlerTypes(commandTypes);
        }
    }
}