using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Utility
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Attempt to get the Description of an IHandlerBase class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
            => GetPublicStaticStringPropertyValue(type, "Description");

        /// <summary>
        /// Attempt to get the Usage of an IHandlerBase class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetUsage(this Type type)
            => GetPublicStaticStringPropertyValue(type, "Usage") ?? GetDescription(type);

        public static string GetGroup(this Type type)
            => GetPublicStaticStringPropertyValue(type, "Group");

        private static string GetPublicStaticStringPropertyValue(Type type, string name)
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (property != null && property.PropertyType == typeof(string))
                return property.GetValue(null) as string;
            return null;
        }

        /// <summary>
        /// Return true if the given verb should display in the Help output for the given handler. False
        /// otherwise.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        public static bool ShouldShowInHelp(this Type type, Verb verb)
        {
            var attrs = type.GetCustomAttributes<VerbAttribute>().ToList();

            // If there are no attributes, we're using a class name and we always show it
            if (!attrs.Any())
                return true;

            return attrs.Any(a => a.Verb.Equals(verb) && !a.Hide);
        }
    }
}
