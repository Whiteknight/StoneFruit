using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Handler source for manually-specified handler instances.  This type is mainly used for
    /// situations where the user doesn't employ a DI container to detect and resolve handler types
    /// automatically
    /// </summary>
    public class NamedInstanceHandlerSource : IHandlerSource
    {
        private readonly VerbTrie<VerbInfo> _verbs;

        public NamedInstanceHandlerSource()
        {
            _verbs = new VerbTrie<VerbInfo>();
        }

        public void Add(Verb verb, IHandlerBase handlerObject, string? description = null, string? usage = null, string? group = null)
        {
            var info = new VerbInfo(verb, handlerObject, description ?? string.Empty, usage ?? string.Empty, group ?? string.Empty);
            _verbs.Insert(verb, info);
        }

        public IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
            => _verbs.Get(arguments).Transform(info => info.HandlerObject);

        public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

        public IResult<IVerbInfo> GetByName(Verb verb) => _verbs.Get(verb);

        public int Count => _verbs.Count;

        private class VerbInfo : IVerbInfo
        {
            public VerbInfo(Verb verb, IHandlerBase handlerObject, string description, string help, string group)
            {
                Verb = verb;
                HandlerObject = handlerObject;
                Description = description;
                Usage = help;
                Group = group;
            }

            public IHandlerBase HandlerObject { get; }
            public Verb Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public string Group { get; }
            public bool ShouldShowInHelp => true;
        }
    }
}
