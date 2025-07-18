using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public interface IValueTypeParser
{
}

public interface IValueTypeParser<out T> : IValueTypeParser
    where T : class
{
    T? TryParse(string value);
}

public static class ValueTypeParserExtensions
{
    public static T? TryParse<T>(this IValueTypeParser<T> parser, IValuedArgument arg)
        where T : class
    {
        if (!arg.Exists())
            return default;
        return NotNull(parser).TryParse(arg.Value);
    }
}
