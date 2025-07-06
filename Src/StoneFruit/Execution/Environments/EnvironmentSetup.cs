using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Sets up the environment mechanism.
/// </summary>
public class EnvironmentSetup : IEnvironmentSetup
{
    private readonly IServiceCollection _services;

    private static readonly IReadOnlyList<string> _defaultNamesList = [
        Constants.EnvironmentNameDefault
    ];

    // TODO: Change this so we don't take IServiceCollection in the constructor.
    // Set instance fields with data as we get it, and then take the IServiceCollection in the
    // BuildUp() method only.
    public EnvironmentSetup(IServiceCollection services)
    {
        _services = services;
    }

    // TODO: Change DI so that we register a CurrentEnvironment object (whose value can be changed
    // from the env handler) and then DI types can inject that value to get environment-specific
    // values for things instead of having all these IEnvironmentFactory<> and instance methods, etc

    public void BuildUp(IServiceCollection services)
    {
        services.AddSingleton<EnvironmentObjectCache>();
        services.TryAddSingleton(new EnvironmentsList(_defaultNamesList));
        services.TryAddSingleton<IEnvironmentCollection>(new InstanceEnvironmentCollection());
    }

    public IEnvironmentSetup SetEnvironments(IReadOnlyList<string> names)
    {
        if (names == null || names.Count == 0)
            names = _defaultNamesList;
        _services.AddSingleton(new EnvironmentsList(names));
        _services.AddSingleton<IEnvironmentCollection, FactoryEnvironmentCollection>();
        return this;
    }

    public IEnvironmentSetup UseFactory<T>(IEnvironmentFactory<T> factory)
        where T : class
    {
        Assert.NotNull(factory);
        _services.AddSingleton(factory);
        _services.AddScoped(services =>
        {
            var environments = services.GetRequiredService<IEnvironmentCollection>();
            var currentEnvName = environments.GetCurrentName()
                .ToResult(() => "Could not get environment context object without valid environment")
                .GetValueOrThrow();

            var objectCache = services.GetRequiredService<EnvironmentObjectCache>();
            var cached = objectCache.Get<T>(currentEnvName);
            if (cached.IsSuccess)
                return cached.GetValueOrThrow();

            var factory = services.GetRequiredService<IEnvironmentFactory<T>>();
            var obj = factory.Create(currentEnvName)
                .ToResult(() => "Could not create valid environment context object for current environment")
                .GetValueOrThrow();
            objectCache.Set(currentEnvName, obj);
            return obj;
        });
        return this;
    }

    public IEnvironmentSetup UseInstance<T>(T environment)
         where T : class
    {
        Assert.NotNull(environment);
        return UseFactory<T>(new InstanceEnvironmentFactory<T>(environment));
    }

    public IEnvironmentSetup None()
    {
        _services.AddSingleton<IEnvironmentCollection>(new InstanceEnvironmentCollection());
        return this;
    }
}
