using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Help;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Metadata;
using StoneFruit.Execution.Scripts;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

#pragma warning disable S1200 // Too many dependencies.

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Setup handlers and handler sources.
/// </summary>
public class HandlerSetup : IHandlerSetup
{
    private readonly ScriptHandlerSource _scripts;
    private readonly NamedInstanceHandlerSource _instances;
    private readonly HashSet<Assembly> _scannedAssemblies;

    public IServiceCollection Services { get; }

    public HandlerSetup(IServiceCollection services)
    {
        _scripts = new ScriptHandlerSource();
        _instances = new NamedInstanceHandlerSource();
        Services = services;
        _scannedAssemblies = [];
    }

    public void BuildUp(IServiceCollection services)
    {
        services.TryAddSingleton(PriorityVerbExtractor.DefaultInstance);
        services.TryAddSingleton<IHandlerMethodInvoker, ServiceProviderMethodInvoker>();
        services.AddSingleton<IHandlerSource, ServiceProviderHandlerSource>();
        services.AddSingleton<IHandlerSource, DelegateHandlerSource>();

        // Register these sources only if they have entries. We don't care about pre-existing
        // registrations, because IHandlerSource is expected to exist in multiples
        if (_scripts.Count > 0)
            services.AddSingleton<IHandlerSource>(_scripts);
        if (_instances.Count > 0)
            services.AddSingleton<IHandlerSource>(_instances);

        // Register built-in handlers
        services.AddHandler<ArgumentDisplayHandler>();
        services.AddHandler<EchoHandler>();
        services.AddHandler<EnvironmentHandler>();
        services.AddHandler<ExitHandler>();
        services.AddHandler<HelpHandler>();
        services.AddHandler<MetadataHandler>();
        services.AddHandler<ShowHandler>();

        // Add the IHandlers, which gets the list of all IHandlerSource instances from the DI
        // This one may be registered by the user already so don't overwrite
        services.TryAddSingleton<IHandlers, HandlerSourceCollection>();
    }

    public IHandlerSetup UseVerbExtractor(IVerbExtractor verbExtractor)
    {
        NotNull(verbExtractor);
        Services.AddSingleton(verbExtractor);
        return this;
    }

    public IHandlerSetup UseMethodInvoker(IHandlerMethodInvoker invoker)
    {
        NotNull(invoker);
        Services.AddSingleton(invoker);
        return this;
    }

    public IHandlerSetup AddSource(Func<IServiceProvider, IHandlerSource> getSource)
    {
        NotNull(getSource);
        Services.AddSingleton(getSource);
        return this;
    }

    public IHandlerSetup Add(Verb verb, Delegate handler, string description = "", string usage = "", string group = "")
    {
        NotNull(verb);
        NotNull(handler);
        Services.AddSingleton(new DelegateHandlerRegistration(handler, verb, description, usage, group));
        return this;
    }

    public IHandlerSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "")
    {
        NotNull(verb);
        NotNull(handler);
        _instances.Add(verb, handler, description, usage, group);
        return this;
    }

    public IHandlerSetup Add<T>(string? prefix = null)
        where T : class, IHandlerBase
    {
        Services.AddHandler<T>(prefix);
        return this;
    }

    public IHandlerSetup Add(Type handlerType, string? prefix = null)
    {
        if (handlerType.IsHandlerType())
        {
            Services.AddHandler(handlerType, prefix);
            return this;
        }

        if (handlerType.IsInstanceMethodHandlersType())
        {
            InstanceMethods.UsePublicInstanceMethodsAsHandlers(this, handlerType, prefix);
            return this;
        }

        throw new EngineBuildException("Type " + handlerType.FullName + " is not a valid handler type");
    }

    public IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "")
    {
        NotNull(verb);
        NotNull(lines);
        _scripts.AddScript(verb, lines, description, usage, group);
        return this;
    }

    public IHandlerSetup ScanAssemblyForHandlers(Assembly? assembly, string? prefix = null)
    {
        if (assembly == null || _scannedAssemblies.Contains(assembly))
            return this;

        // Scan for IHandler and IAsyncHandler implementations
        // Each handler type creates two DI registrations:
        // 1) the type itself, as itself, so we can resolve the handler instance later, and
        // 2) a RegisteredHandler, which holds the handler type, so we can set up verb->handler mappings
        var handlerTypes = NotNull(assembly).GetTypes()
            .Where(t => t.IsPublic && t.IsHandlerType());
        foreach (var type in handlerTypes)
            Services.AddHandler(type, prefix);

        var instanceMethodTypes = assembly.GetTypes()
            .Where(t => t.IsPublic && t.IsInstanceMethodHandlersType());
        foreach (var type in instanceMethodTypes)
            InstanceMethods.UsePublicInstanceMethodsAsHandlers(this, type, prefix);

        _scannedAssemblies.Add(assembly);
        return this;
    }
}
