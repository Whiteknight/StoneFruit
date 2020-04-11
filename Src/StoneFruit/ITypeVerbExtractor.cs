using System;
using System.Collections.Generic;
using StoneFruit.Execution.Handlers;

namespace StoneFruit
{
    /// <summary>
    /// Get a list of possible verbs from an IHandlerBase Type
    /// </summary>
    public interface ITypeVerbExtractor
    {
        /// <summary>
        /// Gets a list of possible verbs from the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Should not return null</returns>
        IReadOnlyList<string> GetVerbs(Type type);
    }

    public static class TypeVerbExtractor
    {
        private static readonly Lazy<ITypeVerbExtractor> _default = new Lazy<ITypeVerbExtractor>(
            () => new PriorityVerbExtractor(
                new VerbAttributeVerbExtractor(),
                new CamelToSpinalNameVerbExtractor()
            )
        );

        /// <summary>
        /// Get the default ITypeVerbExtractor instance which will be used if a custom
        /// one isn't provided.
        /// </summary>
        public static ITypeVerbExtractor DefaultInstance => _default.Value;
    }
}
