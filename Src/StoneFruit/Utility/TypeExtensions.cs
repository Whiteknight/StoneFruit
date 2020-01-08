using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Utility
{
    public static class TypeExtensions
    {
        public static string GetDescription(this Type type)
        {
            return type.GetCustomAttributes<CommandDetailsAttribute>()
                .Select(attr => attr.Description)
                .FirstOrDefault(desc => !string.IsNullOrEmpty(desc));
        }

        public static string GetHelp(this Type type)
        {
            var property = type.GetProperty("Help", BindingFlags.Public | BindingFlags.Static);
            if (property != null)
                return (property.GetValue(null) as string) ?? type.GetDescription() ?? "";
            return type.GetDescription() ?? "";
        }
    }
}
