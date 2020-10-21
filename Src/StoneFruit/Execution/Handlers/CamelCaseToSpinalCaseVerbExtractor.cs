using System;
using System.Collections.Generic;
using System.Reflection;
using ParserObjects;
using StoneFruit.Utility;
using static ParserObjects.Parsers.Specialty.IdentifierParserMethods;

namespace StoneFruit.Execution.Handlers
{
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

            if (string.IsNullOrEmpty(name))
                return new List<Verb>();

            var camelCase = CamelCase();
            var result = camelCase.Parse(name);
            if (!result.Success)
                return new List<Verb>();

            var spinal = string.Join("-", result.Value).ToLowerInvariant();
            return new[] { new Verb(spinal) };
        }
    }
}