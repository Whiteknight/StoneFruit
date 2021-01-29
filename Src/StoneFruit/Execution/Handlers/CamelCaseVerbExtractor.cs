using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ParserObjects;
using StoneFruit.Utility;
using static ParserObjects.ParserMethods;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Verb extractor to derive the verb from the name of the handler class. Common
    /// suffixes are removed ('verb', 'command', 'handler'), CamelCase is converted to
    /// spinal-case, and the name is converted to lower-case.
    /// </summary>
    public class CamelCaseVerbExtractor : IVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (type == null || !typeof(IHandlerBase).IsAssignableFrom(type))
                return Array.Empty<Verb>();

            return GetVerbs(type.Name);
        }

        public IReadOnlyList<Verb> GetVerbs(MethodInfo method) => GetVerbs(method?.Name);

        private IReadOnlyList<Verb> GetVerbs(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Array.Empty<Verb>();

            name = name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler");

            if (string.IsNullOrEmpty(name))
                return Array.Empty<Verb>();

            var camelCase = CamelCase();
            var result = camelCase.Parse(name);
            if (!result.Success)
                return Array.Empty<Verb>();

            var verb = result.Value.Select(s => s.ToLowerInvariant()).ToArray();
            return new[] { new Verb(verb) };
        }
    }
}
