using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class CamelCaseVerbExtractor : IVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (type == null || !typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            return GetVerbs(type.Name);
        }

        public IReadOnlyList<Verb> GetVerbs(MethodInfo method) => GetVerbs(method?.Name);

        private IReadOnlyList<Verb> GetVerbs(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new List<Verb>();

            name = name
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
            // TODO: If parse fails should we return anything?
            if (!result.Success)
                return new[] { s.ToLowerInvariant() };
            return result.Value.Select(s => s.ToLowerInvariant()).ToArray();
        }
    }

    public class CamelCaseToSpinalCaseVerbExtractor : IVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (type == null || !typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            return GetVerbs(type.Name);
        }

        public IReadOnlyList<Verb> GetVerbs(MethodInfo method) => GetVerbs(method?.Name);

        private IReadOnlyList<Verb> GetVerbs(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new List<Verb>();

            name = name
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

            // TODO: If parse fails should we return anything?
            if (!result.Success)
                return new[] { s.ToLowerInvariant() };
            var spinal = string.Join("-", result.Value).ToLowerInvariant();
            return new[] { spinal };
        }
    }
}