using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Arguments
{
    public static class CommandArgumentMapper
    {
        public static T Map<T>(IArguments args)
            where T : new()
        {
            var obj = new T();
            MapOnto(args, obj);
            return obj;
        }

        public static void MapOnto<T>(IArguments args, T obj)
        {
            var targetType = typeof(T);
            var publicProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.SetMethod != null)
                .ToList();

            foreach (var property in publicProperties)
            {
                // Try to map positional argument if the attribute is set
                var attr = property.GetCustomAttributes<ArgumentIndexAttribute>(true).FirstOrDefault();
                if (attr != null)
                {
                    var index = attr.Index;
                    var positionalArgument = args.Consume(index);
                    AssignPropertyValue(positionalArgument, property, obj);
                    continue;
                }

                if ((property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?)) && args.ConsumeFlag(property.Name).Exists())
                {
                    property.SetValue(obj, true);
                    continue;
                }

                var namedArg = args.Consume(property.Name);
                if (namedArg.Exists())
                {
                    AssignPropertyValue(namedArg, property, obj);
                    continue;
                }
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
}