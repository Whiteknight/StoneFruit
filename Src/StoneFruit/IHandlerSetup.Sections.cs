using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;

namespace StoneFruit;

public static class IHandlerSetupSectionsExtensions
{
    public static IHandlerSetup AddSection(this IHandlerSetup setup, string name, Action<HandlerSectionSetup> buildSection)
    {
        buildSection?.Invoke(new HandlerSectionSetup(setup, name));
        return setup;
    }
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
