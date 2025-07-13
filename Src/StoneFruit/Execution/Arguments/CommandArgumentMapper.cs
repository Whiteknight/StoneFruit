using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Map argument values onto the public properties of an object by name.
/// </summary>
public static class CommandArgumentMapper
{
    /// <summary>
    /// Construct a new object instance and map public property values from the
    /// provided arguments.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static T Map<T>(IArguments args)
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
    public static void MapOnto<T>(IArguments args, T obj)
    {
        var targetType = typeof(T);
        var publicProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod != null)
            .ToList();

        foreach (var property in publicProperties)
        {
            // Try to map positional argument if the attribute is set
            var idxAttr = property.GetCustomAttribute<ArgumentIndexAttribute>(true);
            if (idxAttr != null)
            {
                var index = idxAttr.Index;
                var positionalArgument = args.Consume(index);
                if (positionalArgument.Exists())
                {
                    AssignPropertyValue(positionalArgument, property, obj);
                    continue;
                }
            }

            var namedAttr = property.GetCustomAttribute<ArgumentNamedAttribute>(true);
            if (namedAttr != null)
            {
                var name = namedAttr.Name;
                var namedArgument = args.Consume(name);
                if (namedArgument.Exists())
                {
                    AssignPropertyValue(namedArgument, property, obj);
                    continue;
                }
            }

            if ((property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?)) && args.ConsumeFlag(property.Name).Exists())
            {
                property.SetValue(obj, true);
                continue;
            }

            var namedArg = args.Consume(property.Name);
            if (namedArg.Exists())
                AssignPropertyValue(namedArg, property, obj);
        }
    }

    private static void AssignPropertyValue<T>(IValuedArgument argument, PropertyInfo property, T obj)
    {
        if (!argument.Exists())
            return;
        if (property.PropertyType == typeof(string))
            property.SetValue(obj, argument.Value);
        else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            property.SetValue(obj, argument.AsInt());
        else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
            property.SetValue(obj, argument.AsLong());
        else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            property.SetValue(obj, argument.AsBool());
    }
}