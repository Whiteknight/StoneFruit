using System;
using System.Collections.Generic;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Verb extractor which takes the name of the handler class or method, removes common suffixes
    /// ('verb', 'handler', 'command') and converts the remainder to lowercase.
    /// </summary>
    public class ToLowerNameVerbExtractor : IVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (type == null || !typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            return GetVerbs(type.Name);
        }

        public IReadOnlyList<Verb> GetVerbs(MethodInfo method)
        {
            if (method == null)
                return new List<Verb>();

            return GetVerbs(method.Name);
        }

        private IReadOnlyList<Verb> GetVerbs(string name)
        {
            name = name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler")
                .ToLowerInvariant();
            return new[] { new Verb(name) };
        }
    }
}