﻿using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Attempt to get the list of verbs for an IHandlerBase class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

            var name = type.Name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler")
                .CamelCaseToSpinalCase()
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

        /// <summary>
        /// Return true if the given verb should display in the Help output for the given handler. False
        /// otherwise.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
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
