using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Handlers
{
    public class VerbAttributeVerbExtractor : ITypeVerbExtractor
    {
        public IReadOnlyList<string> GetVerbs(Type type)
        {
            if (!typeof(IHandlerBase).IsAssignableFrom(type))
                return new string[0];

            return type.GetCustomAttributes<VerbAttribute>()
                .Select(a => a.CommandName.ToLowerInvariant())
                .Distinct()
                .ToList();
        }
    }
}