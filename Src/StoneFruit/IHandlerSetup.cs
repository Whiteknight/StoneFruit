using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public interface IHandlerSetup
{
    IHandlerSetup ScanAssemblyForHandlers(Assembly? assembly, string? prefix = null);

    IHandlerSetup ScanHandlersFromEntryAssembly(string? prefix = null)
        => ScanAssemblyForHandlers(Assembly.GetEntryAssembly(), prefix);

    IHandlerSetup ScanHandlersFromCurrentAssembly(string? prefix = null)
        => ScanAssemblyForHandlers(Assembly.GetCallingAssembly(), prefix);

    IHandlerSetup ScanHandlersFromAssemblyContaining<T>(string? prefix = null)
        => ScanAssemblyForHandlers(typeof(T).Assembly, prefix);

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
    /// Add a handler source where handlers can be looked up.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    IHandlerSetup AddSource(IHandlerSource source)
    {
        NotNull(source);
        return AddSource(_ => source);
    }

    /// <summary>
    /// Register a handler type. Verbs and help information will be derived from the type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IHandlerSetup Add<T>(string? prefix = null)
        where T : class, IHandlerBase;

    /// <summary>
    /// Register a handler type. Verbs and help information will be derived from the type.
    /// </summary>
    /// <param name="handlerType"></param>
    /// <returns></returns>
    IHandlerSetup Add(Type handlerType, string? prefix = null);

    /// <summary>
    /// Add a function delegate as a handler.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handle"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup Add(Verb verb, Action<IArguments, HandlerContext> handle, string description = "", string usage = "", string group = "");

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
    IHandlerSetup Add(Verb verb, Func<IArguments, HandlerContext, CancellationToken, Task> handleAsync, string description = "", string usage = "", string group = "");

    /// <summary>
    /// Add a script with a verb and zero or more commands to execute.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="lines"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "");

    IHandlerSetup AddSection(string name, Action<HandlerSectionSetup> setup)
    {
        setup?.Invoke(new HandlerSectionSetup(this, name));
        return this;
    }

    /// <summary>
    /// Use the public methods of an instance object as handlers.
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    IHandlerSetup UsePublicInstanceMethodsAsHandlers(object instance, string? group = null)
        => AddSource(provider =>
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

    public HandlerSectionSetup Add(Verb verb, Action<IArguments, HandlerContext> handle, string description = "", string usage = "", string group = "")
    {
        _setup.Add(verb.AddPrefix(_name), handle, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "")
    {
        _setup.Add(verb.AddPrefix(_name), handler, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add(Verb verb, Func<IArguments, HandlerContext, CancellationToken, Task> handleAsync, string description = "", string usage = "", string group = "")
    {
        _setup.Add(verb.AddPrefix(_name), handleAsync, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add<T>()
        where T : class, IHandlerBase
    {
        _setup.Add<T>(_name);
        return this;
    }

    public HandlerSectionSetup Add(Type handlerType)
    {
        _setup.Add(handlerType, _name);
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
