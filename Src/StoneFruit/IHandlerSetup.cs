using System;
using System.Collections.Generic;
using System.Reflection;
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
