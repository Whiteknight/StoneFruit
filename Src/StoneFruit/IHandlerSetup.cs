using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Execution.Handlers;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public interface IHandlerSetup
{
    // TODO: Scan the assembly for handlers
    // TODO: Keep track of which assemblies we scan, to avoid double-scanning
    // TODO: Wrap found handler types in an accessor. Then we can feed the list of accessors into the HandlerSourceCollection and resolve types from there.
    IHandlerSetup ScanAssemblyForHandlers(Assembly assembly);

    /// <summary>
    /// Set the Verb Extractor to use to get verbs from classes and methods where verbs are
    /// not explicitly supplied.
    /// </summary>
    /// <param name="verbExtractor"></param>
    /// <returns></returns>
    IHandlerSetup UseVerbExtractor(IVerbExtractor verbExtractor);

    /// <summary>
    /// Set the Handler Method Invoker which is used to invoke a handler method on an instance
    /// object.
    /// </summary>
    /// <param name="invoker"></param>
    /// <returns></returns>
    IHandlerSetup UseMethodInvoker(IHandlerMethodInvoker invoker);

    /// <summary>
    /// Add a new handler source to the list of sources. Sources are stored in order, sources
    /// added first will be searched first for a matching handler.
    /// </summary>
    /// <param name="getSource"></param>
    /// <returns></returns>
    IHandlerSetup AddSource(Func<IServiceProvider, IHandlerSource> getSource);

    /// <summary>
    /// Add a function delegate as a handler.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handle"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup Add(Verb verb, Action<IArguments, CommandDispatcher> handle, string description = "", string usage = "", string group = "");

    /// <summary>
    /// Add a pre-existing handler instance with the given verb.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handler"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "");

    /// <summary>
    /// Add a function delegate as a handler for asynchronous invocation.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handleAsync"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup AddAsync(Verb verb, Func<IArguments, CommandDispatcher, Task> handleAsync, string description = "", string usage = "", string group = "");

    /// <summary>
    /// Add a script with a verb and zero or more commands to execute.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="lines"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "");

    /// <summary>
    /// Specify an explicit list of handler types to register with the Engine. Notice that these types
    /// may not be constructed using your DI container of choice. If you are using a DI container, you
    /// should try to register types with the container instead.
    /// </summary>
    /// <param name="commandTypes"></param>
    /// <returns></returns>
    IHandlerSetup UseHandlerTypes(IEnumerable<Type> commandTypes);
}

public static class HandlerSetupExtensions
{
    public static IHandlerSetup ScanHandlersFromEntryAssembly(this IHandlerSetup handlers)
        => handlers.ScanAssemblyForHandlers(Assembly.GetExecutingAssembly());

    public static IHandlerSetup ScanHandlersFromCurrentAssembly(this IHandlerSetup handlers)
        => handlers.ScanAssemblyForHandlers(new StackTrace(1).GetFrame(0)!.GetType().Assembly);

    public static IHandlerSetup ScanHandlersFromAssemblyContaining<T>(this IHandlerSetup handlers)
        => handlers.ScanAssemblyForHandlers(typeof(T).Assembly);

    /// <summary>
    /// Add a handler source where handlers can be looked up.
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IHandlerSetup AddSource(this IHandlerSetup handlers, IHandlerSource source)
    {
        NotNull(source);
        return NotNull(handlers).AddSource(_ => source);
    }

    /// <summary>
    /// Use the public methods of an instance object as handlers.
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers(this IHandlerSetup handlers, object instance)
        => NotNull(handlers).AddSource(provider =>
            new InstanceMethodHandlerSource(
                instance,
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                provider.GetRequiredService<IVerbExtractor>()
            )
        );

    /// <summary>
    /// Use the specified list of handler types, with the default instance resolver and
    /// default verb extractor.
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="commandTypes"></param>
    /// <returns></returns>
    public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, params Type[] commandTypes)
        => NotNull(handlers).UseHandlerTypes(NotNull(commandTypes));
}
