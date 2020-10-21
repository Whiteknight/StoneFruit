using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Handlers
{
    public class VerbAttributeVerbExtractor : ITypeVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            return type.GetCustomAttributes<VerbAttribute>()
                .Select(a => a.Verb)
                // TODO V2: Distinct
                .ToList();
        }
    }
}