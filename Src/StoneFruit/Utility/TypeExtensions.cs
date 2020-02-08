using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Utility
{
    public static class StringExtensions
    {
        public static string RemoveSuffix(this string s, string suffix)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            if (s.Length < suffix.Length)
                return s;
            if (s.EndsWith(suffix))
                return s.Substring(0, s.Length - suffix.Length);
            return s;
        }
    }

    public static class TypeExtensions
    {
        public static string GetDescription(this Type type) 
            => GetPublicStaticStringPropertyValue(type, "Description");

        public static string GetUsage(this Type type) 
            => GetPublicStaticStringPropertyValue(type, "Usage") ?? GetDescription(type);

        public static IEnumerable<string> GetVerbs(this Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return Enumerable.Empty<string>();

            var attrs = type.GetCustomAttributes<VerbAttribute>().ToList();
            if (attrs.Any())
            {
                return attrs
                    .Select(a => a.CommandName.ToLowerInvariant())
                    .Distinct();
            }

            // TODO: Would like to convert CamelCase to spinal-case
            var name = type.Name
                .ToLowerInvariant()
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler")
                ;
            return new[] { name };
        }

        private static string GetPublicStaticStringPropertyValue(Type type, string name)
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(string))
                return property.GetValue(null) as string;
            return null;
        }

        public static bool ShouldShowInHelp(this Type type, string verb)
        {
            var attrs = type.GetCustomAttributes<VerbAttribute>().ToList();

            // If there are no attributes, we're using a class name and we always show it
            if (!attrs.Any())
                return true;

            return attrs.Any(a => a.CommandName == verb && a.ShowInHelp);
        }
    }
}
