using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Type so we can register the list of environment names with the container.
/// </summary>
public sealed class EnvironmentsList
{
    public EnvironmentsList(IReadOnlyList<string> validNames)
    {
        ValidNames = validNames == null || validNames.Count == 0
            ? DefaultNamesList
            : validNames;
    }

    public static IReadOnlyList<string> DefaultNamesList => [Constants.EnvironmentNameDefault];

    public IReadOnlyList<string> ValidNames { get; }

    // For larger lists we should use a HashSet instead, but I think most usages will be
    // relatively small.
    public bool Contains(string name) => ValidNames.Contains(name);
}
