using System;
using System.Collections.Generic;
using System.Reflection;
using StoneFruit.Execution.Handlers;

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

    public static class VerbExtractor
    {
        private static readonly Lazy<IVerbExtractor> _default = new Lazy<IVerbExtractor>(
            () => new PriorityVerbExtractor(
                new VerbAttributeVerbExtractor(),
                new CamelCaseVerbExtractor()
            )
        );

        /// <summary>
        /// Get the default ITypeVerbExtractor instance which will be used if a custom
        /// one isn't provided.
        /// </summary>
        public static IVerbExtractor DefaultInstance => _default.Value;
    }
}
