using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Commands
{
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

        public ICommandVerb GetCommandInstance(CompleteCommand command, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            var commandType = _commands.ContainsKey(command.Verb) ? _commands[command.Verb] : null;
            if (commandType == null)
                return null;
            var commandVerb = DuckTypeConstructorInvoker.TryConstruct(commandType, new object[]
            {
                environments,
                environments.Current,
                state,
                output,
                command,
                command.Arguments,
                this
            });
            return commandVerb as ICommandVerb;
        }

        public IReadOnlyDictionary<string, Type> GetAll() => _commands;

        public Type GetCommandTypeByName(string name) 
            => _commands.ContainsKey(name) ? _commands[name] : null;
    }
}
