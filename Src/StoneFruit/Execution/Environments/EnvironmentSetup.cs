using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Sets up the environment mechanism.
/// </summary>
public class EnvironmentSetup : IEnvironmentSetup
{
    private static readonly IReadOnlyList<string> _defaultNamesList = [
        Constants.EnvironmentNameDefault
    ];

    private List<string>? _names;

    public void BuildUp(IServiceCollection services)
    {
        services.TryAddSingleton(new EnvironmentsList(_names ?? _defaultNamesList));
        services.TryAddSingleton<IEnvironmentCollection, EnvironmentCollection>();
        services.AddScoped(p => p.GetRequiredService<IEnvironmentCollection>().GetCurrent());
    }

    public IEnvironmentSetup SetEnvironments(IReadOnlyList<string> names)
    {
        if (names == null || names.Count == 0)
        {
            _names = null;
            return this;
        }

        _names = names.ToList();
        return this;
    }

    public IEnvironmentSetup AddEnvironment(string name)
    {
        _names ??= [];
        if (!_names.Contains(name))
            _names.Add(name);
        return this;
    }
}
