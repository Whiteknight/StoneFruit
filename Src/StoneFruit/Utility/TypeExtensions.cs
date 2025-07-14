using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Utility;

public static class TypeExtensions
{
    /// <summary>
    /// Attempt to get the Description of an IHandlerBase class. Checks static 'Description'
    /// property first, followed by DescriptionAttribute, then
    /// System.ComponentModel.DescriptionAttribute, and finally returns an empty string if no
    /// description is found.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetDescription(this Type type)
        => GetPublicStaticStringPropertyValue(type, "Description")
            ?? GetDescriptionAttributeValue(type)
            ?? string.Empty;

    /// <summary>
    /// Attempt to get the Usage of an IHandlerBase class. Checks static 'Usage' property first,
    /// followed by UsageAttribute and finally returns an empty string if no Usage value is
    /// found.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetUsage(this Type type)
        => GetPublicStaticStringPropertyValue(type, "Usage")
            ?? GetUsageAttributeValue(type)
            ?? GetDescription(type);

    /// <summary>
    /// Get the group of the IHandlerBase class. Checks static 'Group' property first, followed
    /// by GroupAttribute, then System.ComponentModel.CategoryAttribute and finally returns
    /// an empty string if no group is found.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetGroup(this Type type)
        => GetPublicStaticStringPropertyValue(type, "Group")
            ?? GetGroupAttributeValue(type)
            ?? string.Empty;

    private static string? GetPublicStaticStringPropertyValue(Type type, string name)
    {
        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
        return property != null && property.PropertyType == typeof(string)
            ? property.GetValue(null) as string
            : null;
    }

    /// <summary>
    /// Attempts to get a Description value from a custom attribute. Checks the
    /// DescriptionAttribute first, followed by System.ComponentModel.DescriptionAttribute
    /// otherwise. Returns null if no value is found.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? GetDescriptionAttributeValue(this MemberInfo type)
        => type.GetCustomAttribute<DescriptionAttribute>()?.Description
            ?? type.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description;

    /// <summary>
    /// Attempts to get a Usage value from a custom attribute. Checks the UsageAttribute
    /// and returns null if no value is found.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? GetUsageAttributeValue(this MemberInfo type)
        => type.GetCustomAttribute<UsageAttribute>()?.Usage;

    /// <summary>
    /// Attempts to get a Group value from a custom attribute. Checks the GroupAttribute
    /// first, followed by the System.ComponentModel.CategoryAttribute. Returns null if no
    /// value is found.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string? GetGroupAttributeValue(this MemberInfo type)
        => type.GetCustomAttribute<GroupAttribute>()?.Group
            ?? type.GetCustomAttribute<System.ComponentModel.CategoryAttribute>()?.Category;

    /// <summary>
    /// Return true if the given verb should display in the Help output for the given handler. False
    /// otherwise.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="verb"></param>
    /// <returns></returns>
    public static bool ShouldShowInHelp(this Type type, Verb verb)
    {
        var hide = type.GetCustomAttributes<VerbAttribute>().FirstOrDefault(a => a.Verb.Equals(verb))?.Hide;
        if (hide.HasValue)
            return !hide.Value;

        var show = type.GetCustomAttribute<System.ComponentModel.BrowsableAttribute>()?.Browsable;
        if (show.HasValue)
            return show.Value;

        // If there are no attributes specifying otherwise, we show the handler in help by
        // default.
        return true;
    }

    public static bool IsHandlerType(this Type type)
        => type != null && !type.IsAbstract && !type.IsInterface && type.IsAssignableTo(typeof(IHandlerBase));
}
