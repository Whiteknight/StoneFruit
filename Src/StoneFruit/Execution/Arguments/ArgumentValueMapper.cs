using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Arguments;

public class ArgumentValueMapper
{
    /* Provides two basic functions:
     * 1) Map an IValuedArgument to an arbitrary data type, depending on registered IValueTypeParsers
     * 2) Map an IArgumentsCollection to a custom DTO object using property setters, using value mapping from #1
     */
    private static readonly MethodInfo _tryParseValueRawMethod = typeof(ArgumentValueMapper).GetMethod(nameof(TryParseValueRaw), BindingFlags.Public | BindingFlags.Static)!;
    private readonly Dictionary<Type, IValueTypeParser> _typeParsers;

    public ArgumentValueMapper(IEnumerable<IValueTypeParser> typeParsers)
    {
        _typeParsers = typeParsers.OrEmptyIfNull().ToDictionary(v => v.TargetType);
    }

    public static ArgumentValueMapper Empty { get; } = new ArgumentValueMapper([]);

    public Maybe<T> TryParseValue<T>(IValuedArgument arg)
        => TryParseValueRaw<T>(arg, _typeParsers);

    public Maybe<object> TryParseValue(Type t, IValuedArgument arg)
    {
        Debug.Assert(_tryParseValueRawMethod != null, "Raw method should not be null");
        var tryParseMethod = _tryParseValueRawMethod.MakeGenericMethod(t);
        Debug.Assert(tryParseMethod != null, "Constructed method should not be null");
        return tryParseMethod.Invoke(null, [arg, _typeParsers]);
    }

    public static T? TryParseValueRaw<T>(IValuedArgument arg, Dictionary<Type, IValueTypeParser> parsers)
    {
        if (!arg.Exists())
            return default;

        return parsers.TryGetValue(typeof(T), out var mapper) && mapper is IValueTypeParser<T> typed
            ? typed.TryParse(arg)
            : default;
    }

    /// <summary>
    /// Construct a new object instance and map public property values from the
    /// provided arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public T Map<T>(IArguments args)
        where T : new()
    {
        var obj = new T();
        MapOnto(args, obj);
        return obj;
    }

    /// <summary>
    /// Map from the provided arguments onto the public properties of an existing
    /// object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="obj"></param>
    public void MapOnto<T>(IArguments args, T obj)
    {
        var targetType = typeof(T);
        var publicProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod != null)
            .ToList();

        foreach (var property in publicProperties)
        {
            var arg = FindSuitablePositionalArgument(property, args)
                .Or(() => FindSuitableNamedArgument(property, args));
            if (arg.IsSuccess)
            {
                AssignPropertyFromValuedArgument(arg.GetValueOrThrow(), property, obj);
                continue;
            }

            if (IsBoolSettableFromFlag(args, property))
                property.SetValue(obj, true);
        }
    }

    private static Maybe<IValuedArgument> FindSuitablePositionalArgument(PropertyInfo property, IArgumentCollection args)
    {
        var idxAttr = property.GetCustomAttribute<ArgumentIndexAttribute>(true);
        if (idxAttr != null)
        {
            var index = idxAttr.Index;
            var positionalArgument = args.Consume(index);
            if (positionalArgument.Exists())
                return new Maybe<IValuedArgument>(positionalArgument);
        }

        return default;
    }

    private static Maybe<IValuedArgument> FindSuitableNamedArgument(PropertyInfo property, IArgumentCollection args)
    {
        var namedAttr = property.GetCustomAttribute<ArgumentNamedAttribute>(true);
        if (namedAttr != null)
        {
            var namedArgument = args.Consume(namedAttr.Name, false);
            if (namedArgument.Exists())
                return new Maybe<IValuedArgument>(namedArgument);
        }

        var namedArg = args.Consume(property.Name, false);
        if (namedArg.Exists())
            return new Maybe<IValuedArgument>(namedArg);

        return default;
    }

    private static bool IsBoolSettableFromFlag(IArgumentCollection args, PropertyInfo property)
        => (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            && args.ConsumeFlag(property.Name, false).Exists();

    private void AssignPropertyFromValuedArgument<T>(IValuedArgument argument, PropertyInfo property, T obj)
    {
        if (!argument.Exists())
            return;

        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
        {
            property.SetValue(obj, argument.AsInt());
            return;
        }

        if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
        {
            property.SetValue(obj, argument.AsLong());
            return;
        }

        if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
        {
            property.SetValue(obj, argument.AsBool());
            return;
        }

        var value = TryParseValue(property.PropertyType, argument);
        value.OnSuccess(v => property.SetValue(obj, v));
    }
}
