using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Trie;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// A handler source which takes a list of Type and attempts to construct one using
    /// built-in mechanisms. This source is used primarily for built-in handlers and for situations
    /// where the user is not employing a DI container to find and resolve handlers automatically.
    /// </summary>
    public class TypeListConstructSource : IHandlerSource
    {
        private readonly TypeInstanceResolver _resolver;
        private readonly VerbTrie<VerbInfo> _types;

        public TypeListConstructSource(IEnumerable<Type> types, TypeInstanceResolver resolver, IVerbExtractor verbExtractor)
        {
            Assert.NotNull(types, nameof(types));
            Assert.NotNull(resolver, nameof(resolver));

            _resolver = resolver;

            _types = new VerbTrie<VerbInfo>();
            foreach (var commandType in types)
            {
                var verbs = verbExtractor.GetVerbs(commandType);
                foreach (var verb in verbs)
                    _types.Insert(verb, new VerbInfo(verb, commandType));
            }
        }

        public IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var verbInfo = _types.Get(arguments);
            if (!verbInfo.HasValue)
                return FailureResult<IHandlerBase>.Instance;
            var instance = ResolveInstance(arguments, dispatcher, verbInfo.Value.Type);
            return instance == null ? FailureResult<IHandlerBase>.Instance : new SuccessResult<IHandlerBase>(instance);
        }

        private IHandlerBase? ResolveInstance(IArguments command, CommandDispatcher dispatcher, Type commandType)
            => _resolver(commandType, command, dispatcher) as IHandlerBase;

        public IEnumerable<IVerbInfo> GetAll() => _types.GetAll().Select(kvp => kvp.Value);

        public IResult<IVerbInfo> GetByName(Verb verb) => _types.Get(verb).Transform(i => (IVerbInfo)i);

        private class VerbInfo : IVerbInfo
        {
            public VerbInfo(Verb verb, Type type)
            {
                Verb = verb;
                Type = type;
            }

            public Type Type { get; }

            public Verb Verb { get; }

            public string Group => Type.GetGroup();
            public string Description => Type.GetDescription();
            public string Usage => Type.GetUsage();
            public bool ShouldShowInHelp => Type.ShouldShowInHelp(Verb);
        }
    }
}
