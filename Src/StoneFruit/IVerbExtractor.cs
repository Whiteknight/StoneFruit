using System;
using System.Collections.Generic;
using System.Reflection;

namespace StoneFruit
{
    /// <summary>
    /// Get a list of possible verbs from an IHandlerBase Type
    /// </summary>
    public interface IVerbExtractor
    {
        /// <summary>
        /// Gets a list of possible verbs from the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Should not return null</returns>
        IReadOnlyList<Verb> GetVerbs(Type type);
        IReadOnlyList<Verb> GetVerbs(MethodInfo method);
    }
}
