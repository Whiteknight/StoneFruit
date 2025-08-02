using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public interface IHandlerSetup
{
    // TODO: If we add a prefix here, we can scan an entire assembly into a prefix.
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

    // TODO: If RegisteredHandler adds some metadata for prefix and group, we can use Add<T>()
    // and Add(Type) methods to HandlerSectionSetup as well.
    /// <summary>
    /// Register a handler type. Verbs and help information will be derived from the type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IHandlerSetup Add<T>()
        where T : class, IHandlerBase;

    /// <summary>
    /// Register a handler type. Verbs and help information will be derived from the type.
    /// </summary>
    /// <param name="handlerType"></param>
    /// <returns></returns>
    IHandlerSetup Add(Type handlerType);

    /// <summary>
    /// Add a function delegate as a handler.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handle"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup Add(Verb verb, Action<HandlerContext> handle, string description = "", string usage = "", string group = "");

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
    IHandlerSetup Add(Verb verb, Func<HandlerContext, CancellationToken, Task> handleAsync, string description = "", string usage = "", string group = "");

    /// <summary>
    /// Add a script with a verb and zero or more commands to execute.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="lines"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "");
}

public static class HandlerSetupExtensions
{
    public static IHandlerSetup ScanHandlersFromEntryAssembly(this IHandlerSetup handlers)
        => handlers.ScanAssemblyForHandlers(Assembly.GetExecutingAssembly());

    public static IHandlerSetup ScanHandlersFromCurrentAssembly(this IHandlerSetup handlers)
        => handlers.ScanAssemblyForHandlers(new StackTrace(1).GetFrame(0)!.GetType().Assembly);

    public static IHandlerSetup ScanHandlersFromAssemblyContaining<T>(this IHandlerSetup handlers)
        => handlers.ScanAssemblyForHandlers(typeof(T).Assembly);

    public static IHandlerSetup AddSection(this IHandlerSetup handlers, string name, Action<HandlerSectionSetup> setup)
    {
        setup?.Invoke(new HandlerSectionSetup(handlers, name));
        return handlers;
    }

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
    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers(this IHandlerSetup handlers, object instance, string? group = null)
        => NotNull(handlers).AddSource(provider =>
            new InstanceMethodHandlerSource(
                instance,
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                provider.GetRequiredService<IVerbExtractor>(),
                group
            )
        );
}

public sealed class HandlerSectionSetup
{
    private readonly IHandlerSetup _setup;
    private readonly string _name;

    public HandlerSectionSetup(IHandlerSetup setup, string name)
    {
        _setup = setup;
        _name = name;
    }

    public HandlerSectionSetup Add(Verb verb, Action<HandlerContext> handle, string description = "", string usage = "", string group = "")
    {
        _setup.Add(verb.AddPrefix(_name), handle, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "")
    {
        _setup.Add(verb.AddPrefix(_name), handler, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add(Verb verb, Func<HandlerContext, CancellationToken, Task> handleAsync, string description = "", string usage = "", string group = "")
    {
        _setup.Add(verb.AddPrefix(_name), handleAsync, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "")
    {
        _setup.AddScript(verb.AddPrefix(_name), lines, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup UsePublicInstanceMethodsAsHandlers(object instance)
    {
        var name = _name;
        _setup.AddSource(provider =>
            new InstanceMethodHandlerSource(
                instance,
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                new PrefixingVerbExtractor(name, provider.GetRequiredService<IVerbExtractor>()),
                name
            )
        );
        return this;
    }

    private string GetGroup(string group)
        => string.IsNullOrEmpty(group) ? _name : group;
}
