using System;
using System.Reflection;

namespace StoneFruit;

/// <summary>
/// Get a list of possible verbs from an IHandlerBase Type.
/// </summary>
public interface IVerbExtractor
{
    /// <summary>
    /// Gets a list of possible verbs from the type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>Should not return null.</returns>
    Result<Verb[], Verb.Error> GetVerbs(Type type);

    /// <summary>
    /// Gets a list of possible verbs from the method.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    Result<Verb[], Verb.Error> GetVerbs(MethodInfo method);
}
