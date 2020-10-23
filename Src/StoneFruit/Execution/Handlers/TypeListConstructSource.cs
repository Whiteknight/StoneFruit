using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// A command source which takes a list of Type and attempts to construct one using
    /// built-in mechanisms
    /// </summary>
    public class TypeListConstructSource : IHandlerSource
    {
        private readonly TypeInstanceResolver _resolver;
        private readonly VerbTrie<VerbInfo> _types;

        public TypeListConstructSource(IEnumerable<Type> commandTypes, TypeInstanceResolver resolver, IVerbExtractor verbExtractor)
        {
            _resolver = resolver ?? DefaultResolver;
            verbExtractor ??= PriorityVerbExtractor.DefaultInstance;
            _types = new VerbTrie<VerbInfo>();
            foreach (var commandType in commandTypes)
            {
                var verbs = verbExtractor.GetVerbs(commandType);
                foreach (var verb in verbs)
                    _types.Insert(verb, new VerbInfo(verb, commandType));
            }
        }

        private static object DefaultResolver(Type commandType, IArguments arguments, CommandDispatcher dispatcher)
        {
            return DuckTypeConstructorInvoker.TryConstruct(commandType, new[]
            {
                // long-lived objects
                dispatcher,
                dispatcher.Environments,
                dispatcher.State,
                dispatcher.Output,
                dispatcher.Parser,
                dispatcher.Handlers,

                // transient objects
                dispatcher.Environments.Current,
                arguments
            });
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var verbInfo = _types.Get(arguments);
            if (verbInfo?.Type == null)
                return null;
            return ResolveInstance(arguments, dispatcher, verbInfo.Type);
        }

        public IHandlerBase GetInstance<TCommand>(IArguments command, CommandDispatcher dispatcher)
            where TCommand : class, IHandlerBase
            => ResolveInstance(command, dispatcher, typeof(TCommand));

        private IHandlerBase ResolveInstance(IArguments command, CommandDispatcher dispatcher, Type commandType)
            => _resolver(commandType, command, dispatcher) as IHandlerBase;

        public IEnumerable<IVerbInfo> GetAll() => _types.GetAll().Select(kvp => kvp.Value);

        public IVerbInfo GetByName(Verb verb) => _types.Get(verb);

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
