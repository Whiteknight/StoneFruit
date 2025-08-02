using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

public static class VerbStringExtensions
{
    public static string CleanVerbName(this string name)
        => (name ?? string.Empty)
            .RemoveSuffix("verb")
            .RemoveSuffix("command")
            .RemoveSuffix("handler");
}
