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
    IServiceCollection Services { get; }

    /// <summary>
    /// Scan the given Assembly for handler types marked with IHandler or IAsyncHandler.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    IHandlerSetup ScanAssemblyForHandlers(Assembly? assembly, string? prefix = null);

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
    /// If the type is an IHandler or IAsyncHandler it will be registered.
    /// If the type is IInstanceMethodHandlers each method on the object will be registered as a handler.
    /// </summary>
    /// <param name="handlerType"></param>
    /// <returns></returns>
    IHandlerSetup Add(Type handlerType, string? prefix = null);

    /// <summary>
    /// Add a function delegate as a handler.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="handler"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup Add(Verb verb, Delegate handler, string description = "", string usage = "", string group = "");

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
    /// Add a script with a verb and zero or more commands to execute.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="lines"></param>
    /// <param name="description"></param>
    /// <param name="usage"></param>
    /// <returns></returns>
    IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "");
}

/// <summary>
/// Base type for handler objects.
/// </summary>
public interface IHandlerBase
{
    static virtual string Description { get; } = string.Empty;
    static virtual string Usage { get; } = string.Empty;
    static virtual string Group { get; } = string.Empty;
}

/// <summary>
/// A handler. Handlers are invoked in response to specific verbs depending on
/// registrations and mapping rules. Any dependencies of the handler are expected to
/// be injected in the handler during creation.
/// </summary>
public interface IHandler : IHandlerBase
{
    /// <summary>
    /// Execute the handler and perform any work.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="context"></param>
    void Execute(IArguments arguments, HandlerContext context);
}

/// <summary>
/// An asynchronous handler. Handlers are invoked in response to specific verbs
/// depending on registrations and mapping rules. Any dependencies of the handler are
/// expected to be injected in the handler during creation.
/// </summary>
public interface IAsyncHandler : IHandlerBase
{
    /// <summary>
    /// Execute the handler asynchronously and perform any work.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation);
}

/// <summary>
/// Represents all the handlers known by the system. Handlers may be of different types and
/// come from a variety of different sources.
/// </summary>
public interface IHandlers
{
    /// <summary>
    /// Given a list of arguments, find a handler with a matching verb. Returns null if no
    /// matching handler is found.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Maybe<IHandlerBase> GetInstance(HandlerContext context);

    /// <summary>
    /// Return help information for all handlers. Used mostly by the  "help" handler to display
    /// help information.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IVerbInfo> GetAll();

    /// <summary>
    /// Get detailed information about a specific handler by verb. Returns null if a handler
    /// is not registered with that verb.
    /// </summary>
    /// <param name="verb"></param>
    /// <returns></returns>
    Maybe<IVerbInfo> GetByName(Verb verb);
}

/// <summary>
/// Manages a list of available handlers, instantiates handlers on request, and
/// provides metadata about the registered verbs.
/// </summary>
public interface IHandlerSource
{
    /// <summary>
    /// Instantiates the handler for the command. If a suitable handler is not found
    /// null is returned.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Maybe<IHandlerBase> GetInstance(HandlerContext context);

    /// <summary>
    /// Get metadata information about all verbs registered with this source.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IVerbInfo> GetAll();

    /// <summary>
    /// Get metadata information about a single verb. If the verb is not registered
    /// with this source, null is returned.
    /// </summary>
    /// <param name="verb"></param>
    /// <returns></returns>
    Maybe<IVerbInfo> GetByName(Verb verb);
}
