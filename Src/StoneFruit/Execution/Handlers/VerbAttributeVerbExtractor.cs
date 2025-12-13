using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Extracts a list of Verbs from VerbAttribute annotations on the class or method.
/// </summary>
public class VerbAttributeVerbExtractor : IVerbExtractor
{
    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type.IsHandlerType()
            ? GetVerbsInternal(type.GetCustomAttributes<VerbAttribute>())
            : Verb.InvalidHandler;

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => method == null
            ? Verb.InvalidHandler
            : GetVerbsInternal(method.GetCustomAttributes<VerbAttribute>());

    private static Result<Verb[], Verb.Error> GetVerbsInternal(IEnumerable<VerbAttribute> attrs)
        => attrs
            .Select(a => Verb.TryCreate(a.Verb))
            .Aggregate(new List<Verb>(), (l, r) =>
            {
                r.OnSuccess(verb => l.Add(verb));
                return l;
            })
            .ToArray();
}
