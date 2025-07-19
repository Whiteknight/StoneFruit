using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Arguments;
public class ArgumentValueMapper
{
    private static readonly MethodInfo _tryParseValueRawMethod = typeof(ArgumentValueMapper).GetMethod(nameof(TryParseValueRaw), BindingFlags.Public | BindingFlags.Static)!;
    private readonly IValueTypeParser[] _typeParsers;

    public ArgumentValueMapper(IEnumerable<IValueTypeParser> typeParsers)
    {
        _typeParsers = typeParsers.OrEmptyIfNull().ToArray();
    }

    public static ArgumentValueMapper Empty { get; } = new ArgumentValueMapper([]);

    public Maybe<T> TryParseValue<T>(string value)
        where T : class
    {
        return TryParseValueRaw<T>(value, _typeParsers);
    }

    public Maybe<object> TryParseValue(Type t, string value)
    {
        Debug.Assert(_tryParseValueRawMethod != null, "Raw method should not be null");
        var tryParseMethod = _tryParseValueRawMethod.MakeGenericMethod(t);
        Debug.Assert(tryParseMethod != null, "Constructed method should not be null");
        return tryParseMethod.Invoke(null, [value, _typeParsers]);
    }

    public static T? TryParseValueRaw<T>(string value, IValueTypeParser[] parsers)
        where T : class
    {
        return parsers
            .OfType<IValueTypeParser<T>>()
            .FirstOrDefault()
            ?.TryParse(value);
    }
}
