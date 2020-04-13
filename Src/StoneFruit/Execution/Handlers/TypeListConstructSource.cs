using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// A command source which takes a list of Type and attempts to construct one using
    /// built-in mechanisms
    /// </summary>
    public class TypeListConstructSource : IHandlerSource
    {
        private readonly IReadOnlyDictionary<string, Type> _commands;

        public TypeListConstructSource(IEnumerable<Type> commandTypes, ITypeVerbExtractor verbExtractor)
        {
            verbExtractor ??= TypeVerbExtractor.DefaultInstance;
            _commands = commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType =>
                    verbExtractor.GetVerbs(commandType)
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher) 
            => _commands.ContainsKey(command.Verb) ? ResolveInstance(command, dispatcher, _commands[command.Verb]) : null;

        public IHandlerBase GetInstance<TCommand>(Command command, CommandDispatcher dispatcher)
            where TCommand : class, IHandlerBase
            => ResolveInstance(command, dispatcher, typeof(TCommand));

        private IHandlerBase ResolveInstance(Command command, CommandDispatcher dispatcher, Type commandType)
        {
            var commandVerb = DuckTypeConstructorInvoker.TryConstruct(commandType, new[]
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
                command,
                command.Arguments
            });
            return commandVerb as IHandlerBase;
        }

        public IEnumerable<IVerbInfo> GetAll() 
            => _commands.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _commands.ContainsKey(name) ? new VerbInfo(name, _commands[name]) : null;

        private class VerbInfo : IVerbInfo
        {
            private readonly Type _type;

            public VerbInfo(string verb, Type type)
            {
                Verb = verb;
                _type = type;
            }

            public string Verb { get; }
            public string Description => _type.GetDescription();
            public string Usage => _type.GetUsage();
            public bool ShouldShowInHelp => _type.ShouldShowInHelp(Verb);
        }
    }
}
