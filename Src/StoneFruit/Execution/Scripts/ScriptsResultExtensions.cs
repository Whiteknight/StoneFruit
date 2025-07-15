using System.Collections.Generic;

namespace StoneFruit.Execution.Scripts;

public static class ScriptsResultExtensions
{
    public static List<ArgumentsOrString> ThrowIfContainsErrors(this Result<List<ArgumentsOrString>, ScriptsError> result)
        => result.Match(
            success => success,
            error => throw new ScriptsException(error));
}
