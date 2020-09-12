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
    public class CamelToSpinalNameVerbExtractor : ITypeVerbExtractor
    {
        public IReadOnlyList<string> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new string[0];

            var name = type.Name
                .RemoveSuffix("verb")
                .RemoveSuffix("command")
                .RemoveSuffix("handler");
            name = CamelCaseToSpinalCase(name);
            return new[] { name };
        }

        private static string CamelCaseToSpinalCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            var camelCase = CamelCase();
            var result = camelCase.Parse(s);
            if (!result.Success)
                return s.ToLowerInvariant();
            return string.Join("-", result.Value.Select(s => s.ToLowerInvariant()));
        }
    }
}