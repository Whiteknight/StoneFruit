using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.VerbSources
{
    /// <summary>
    /// A command source which takes a list of Type and attempts to construct one using built-in mechanisms
    /// </summary>
    public class TypeListConstructCommandSource : ICommandVerbSource
    {
        private readonly IReadOnlyDictionary<string, Type> _commands;

        public TypeListConstructCommandSource(IEnumerable<Type> commandTypes)
        {
            _commands = commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType => commandType
                    .GetVerbs()
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public ICommandVerb GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            var commandType = _commands.ContainsKey(completeCommand.Verb) ? _commands[completeCommand.Verb] : null;
            return commandType == null ? null : ResolveInstance(completeCommand, dispatcher, commandType);
        }

        public ICommandVerb GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher)
            where TCommand : class, ICommandVerb
            => ResolveInstance(completeCommand, dispatcher, typeof(TCommand));

        private ICommandVerb ResolveInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher, Type commandType)
        {
            var commandVerb = DuckTypeConstructorInvoker.TryConstruct(commandType, new[]
            {
                // long-lived objects
                dispatcher,
                dispatcher.Environments,
                dispatcher.State,
                dispatcher.Output,
                dispatcher.Parser,

                // transient objects
                dispatcher.Environments.Current,
                completeCommand,
                completeCommand.Arguments,
                this
            });
            return commandVerb as ICommandVerb;
        }

        public IEnumerable<IVerbInfo> GetAll() => _commands.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

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
            public string Help => _type.GetUsage();
            public bool ShouldShowInHelp => _type.ShouldShowInHelp(Verb);
        }
    }
}
