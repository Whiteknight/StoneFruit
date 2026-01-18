using System;

namespace StoneFruit;

public interface IValueTypeParser
{
    Type TargetType { get; }
}

public interface IValueTypeParser<out T> : IValueTypeParser
{
    T? TryParse(IValuedArgument value);
}
