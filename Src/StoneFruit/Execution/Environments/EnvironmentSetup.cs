using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Execution.Exceptions;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Sets up the environment mechanism.
/// </summary>
public class EnvironmentSetup : IEnvironmentSetup
{
    private List<string>? _names;

    public void BuildUp(IServiceCollection services)
    {
        services.TryAddSingleton<IEnvironments>(p => new EnvironmentCollection(_names));
        services.AddScoped(p => p.GetRequiredService<IEnvironments>()
            .GetCurrent()
            .Match(e => e, err => throw EnvironmentNotSetException.Create(err)));
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
