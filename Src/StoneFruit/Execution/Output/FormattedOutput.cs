namespace StoneFruit.Execution.Output;

public static class FormattedOutputExtensions
{
    public static IOutput Format<T>(this IOutput output, string format, T value)
    {
        // TODO: Need to parse the format string into a format template
        // TODO: Need to apply the template to the value to get string output
        // TODO: Need to write that string to the output
        return output;
    }

    public static IOutput FormatLine<T>(this IOutput output, string format, T value)
        => Format(output, format, value).WriteLine();
}

public abstract record FormatTemplateItem;
public record FormatTemplateLiteral(string Value) : FormatTemplateItem;
public record FormatTemplateProperty(string Name) : FormatTemplateItem;