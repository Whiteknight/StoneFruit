using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Extracts a list of Verbs from VerbAttribute annotations on the class or method
    /// </summary>
    public class VerbAttributeVerbExtractor : IVerbExtractor
    {
        public IReadOnlyList<Verb> GetVerbs(Type type)
        {
            if (type == null || !typeof(IHandlerBase).IsAssignableFrom(type))
                return new List<Verb>();

            return type.GetCustomAttributes<VerbAttribute>()
                .Select(a => a.Verb)
                .ToList();
        }

        public IReadOnlyList<Verb> GetVerbs(MethodInfo method)
        {
            if (method == null)
                return new List<Verb>();

            return method.GetCustomAttributes<VerbAttribute>()
                .Select(a => a.Verb)
                .ToList();
        }
    }
}