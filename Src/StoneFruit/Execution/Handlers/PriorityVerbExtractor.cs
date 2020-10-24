using System;
using System.Collections.Generic;
using System.Reflection;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Takes an array of type verb extractors and attempts to invoke each one in sequence
    /// until a verb is successfully found.
    /// </summary>
    public class PriorityVerbExtractor : IVerbExtractor
    {
        // The default set of extractors. First we look for VerbAttribute, then we attempt to parse
        // the name as CamelCase, finally we just take the name and lowercase it.
        private static readonly Lazy<IVerbExtractor> _default = new Lazy<IVerbExtractor>(
            () => new PriorityVerbExtractor(
                new VerbAttributeVerbExtractor(),
                new CamelCaseVerbExtractor(),
                new ToLowerNameVerbExtractor()
            )
        );

        private readonly IVerbExtractor[] _extractors;

        public PriorityVerbExtractor(params IVerbExtractor[] extractors)
        {
            _extractors = extractors;
        }

        /// <summary>
        /// Get the default ITypeVerbExtractor instance which will be used if a custom
        /// one isn't provided.
        /// </summary>
        public static IVerbExtractor DefaultInstance => _default.Value;

        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (type == null || !typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();
            foreach (var extractor in _extractors)
            {
                var verbs = extractor.GetVerbs(type);
                if (verbs != null && verbs.Count > 0)
                    return verbs;
            }
            return new List<Verb>();
        }

        public IReadOnlyList<Verb> GetVerbs(MethodInfo method)
        {
            if (method == null)
                return new List<Verb>();
            foreach (var extractor in _extractors)
            {
                var verbs = extractor.GetVerbs(method);
                if (verbs != null && verbs.Count > 0)
                    return verbs;
            }
            return new List<Verb>();
        }
    }
}