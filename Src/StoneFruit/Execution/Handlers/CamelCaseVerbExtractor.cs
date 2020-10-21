using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.Utility;
using static ParserObjects.Parsers.Specialty.IdentifierParserMethods;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Verb extractor to derive the verb from the name of the handler class. Common
    /// suffixes are removed ('verb', 'command', 'handler'), CamelCase is converted to
    /// spinal-case, and the name is converted to lower-case.
    /// </summary>
    public class CamelCaseVerbExtractor : ITypeVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            var name = type.Name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler");
            var verb = GetVerb(name);
            return new[] { new Verb(verb) };
        }

        private static string[] GetVerb(string s)
        {
            if (string.IsNullOrEmpty(s))
                return new[] { string.Empty };

            var camelCase = CamelCase();
            var result = camelCase.Parse(s);
            if (!result.Success)
                return new[] { s.ToLowerInvariant() };
            return result.Value.Select(s => s.ToLowerInvariant()).ToArray();
        }
    }
}