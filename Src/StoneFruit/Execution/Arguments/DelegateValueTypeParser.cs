using System;

namespace StoneFruit.Execution.Arguments;

public sealed class DelegateValueTypeParser<T> : IValueTypeParser<T>
    where T : class
{
    private readonly Func<string, T> _parse;

    public DelegateValueTypeParser(Func<string, T> parse)
    {
        _parse = parse;
    }

    public T? TryParse(string value) => _parse(value);
}
