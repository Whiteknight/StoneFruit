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
        public IReadOnlyList<string> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new string[0];

            var name = type.Name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler")
                .ToLowerInvariant();
            return new[] { name };
        }
    }
}