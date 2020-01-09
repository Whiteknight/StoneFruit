using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Utility
{
    public static class TypeExtensions
    {
        public static string GetDescription(this Type type)
        {
            return GetPublicStaticStringPropertyValue(type, "Description");
        }


        public static string GetHelp(this Type type)
        {
            return GetPublicStaticStringPropertyValue(type, "Help") ?? GetDescription(type);
        }

        private static string GetPublicStaticStringPropertyValue(Type type, string name)
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(string))
                return property.GetValue(null) as string;
            return null;
        }
    }
}
