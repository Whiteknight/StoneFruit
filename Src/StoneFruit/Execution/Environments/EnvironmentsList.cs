using System.Collections.Generic;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Type so we can register the list of environment names with the container
/// </summary>
public sealed class EnvironmentsList
{
    public EnvironmentsList(IReadOnlyList<string> validNames)
    {
        ValidNames = validNames;
    }

    public IReadOnlyList<string> ValidNames { get; }
}
