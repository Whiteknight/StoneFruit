using System;
using System.Collections.Generic;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Verb extractor which takes the name of the handler class, removes common suffixes
    /// ('verb', 'handler', 'command') and converts the remainder to lowercase.
    /// </summary>
    public class ToLowerNameVerbExtractor : ITypeVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            var name = type.Name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler")
                .ToLowerInvariant();
            return new[] { new Verb(name) };
        }
    }
}