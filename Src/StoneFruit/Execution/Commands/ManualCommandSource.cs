using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var nameMap = new Dictionary<string, Type>();
            foreach (var commandType in commandTypes)
            {
                if (!typeof(ICommandVerb).IsAssignableFrom(commandType))
                    continue;

                var attrs = commandType.GetCustomAttributes<CommandDetailsAttribute>().ToList();
                foreach (var attr in attrs)
                {
                    var name = attr.CommandName.ToLowerInvariant();
                    if (nameMap.ContainsKey(name))
                        continue;
                    nameMap.Add(name, commandType);
                }

                if (attrs.Count == 0)
                {
                    var name = commandType.Name.ToLowerInvariant();
                    if (name.EndsWith("verb"))
                        name = name.Substring(0, name.Length - 4);
                    if (name.EndsWith("command"))
                        name = name.Substring(0, name.Length - 7);
                    nameMap.Add(name, commandType);
                }
            }

            _commands = nameMap;
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
                dispatcher,
                dispatcher.Environments,
                dispatcher.Environments.Current,
                dispatcher.State,
                dispatcher.Output,
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
