using System;

namespace StoneFruit.Execution.Arguments;

public sealed class DelegateValueTypeParser<T> : IValueTypeParser<T>
{
    private readonly Func<IValuedArgument, T> _parse;

    public DelegateValueTypeParser(Func<IValuedArgument, T> parse)
    {
        _parse = parse;
    }

    public Type TargetType => typeof(T);

    public T? TryParse(IValuedArgument value) => _parse(value);
}
