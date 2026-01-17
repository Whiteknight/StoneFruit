using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
    public HandlerSectionSetup(IHandlerSetup setup, string name)
    {
        Handlers = setup;
        Name = name;
    }

    public IServiceCollection Services => Handlers.Services;

    public IHandlerSetup Handlers { get; }

    public string Name { get; }

    public HandlerSectionSetup Add(Verb verb, Action<IArguments, HandlerContext> handle, string description = "", string usage = "", string group = "")
    {
        Handlers.Add(verb.AddPrefix(Name), handle, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "")
    {
        Handlers.Add(verb.AddPrefix(Name), handler, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add(Verb verb, Func<IArguments, HandlerContext, CancellationToken, Task> handleAsync, string description = "", string usage = "", string group = "")
    {
        Handlers.Add(verb.AddPrefix(Name), handleAsync, description, usage, GetGroup(group));
        return this;
    }

    public HandlerSectionSetup Add<T>()
        where T : class, IHandlerBase
    {
        Handlers.Add<T>(Name);
        return this;
    }

    public HandlerSectionSetup Add(Type handlerType)
    {
        Handlers.Add(handlerType, Name);
        return this;
    }

    public HandlerSectionSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "")
    {
        Handlers.AddScript(verb.AddPrefix(Name), lines, description, usage, GetGroup(group));
        return this;
    }

    private string GetGroup(string group)
        => string.IsNullOrEmpty(group) ? Name : group;
}
