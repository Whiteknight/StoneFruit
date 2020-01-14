using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Commands
{
    /// <summary>
    /// A command source where the commands are added through method calls instead of found dynamically
    /// </summary>
    public class ManualCommandSource : ICommandSource
    {
        private readonly IReadOnlyDictionary<string, Type> _commands;

        public ManualCommandSource(IEnumerable<Type> commandTypes)
        {
            _commands = commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType => commandType
                    .GetVerbs()
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public ICommandVerb GetCommandInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            var commandType = _commands.ContainsKey(completeCommand.Verb) ? _commands[completeCommand.Verb] : null;
            return commandType == null ? null : ResolveInstance(completeCommand, dispatcher, commandType);
        }

        public ICommandVerb GetCommandInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
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

        public IReadOnlyDictionary<string, Type> GetAll() => _commands;

        public Type GetCommandTypeByName(string name) 
            => _commands.ContainsKey(name) ? _commands[name] : null;
    }
}
