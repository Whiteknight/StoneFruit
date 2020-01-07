using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StoneFruit.Execution;
using StoneFruit.Execution.Commands;
using StructureMap;

namespace StoneFruit.StructureMap
{
    public class StructureMapCommandSource : ICommandSource
    {
        private readonly IContainer _container;
        private readonly IReadOnlyDictionary<string, CommandTypeAndDescription> _nameMap;

        private class CommandTypeAndDescription
        {
            public CommandTypeAndDescription(Type type, string description)
            {
                Type = type;
                Description = description;
            }

            public Type Type { get; }
            public string Description { get; }
        }

        public StructureMapCommandSource()
        {
            var container = new Container();
            container.Configure(c => c.ScanForCommandVerbs());
            _container = container;
            _nameMap = SetupNameMapping();
        }

        public StructureMapCommandSource(IContainer container)
        {
            _container = container;
            _nameMap = SetupNameMapping();
        }

        private IReadOnlyDictionary<string, CommandTypeAndDescription> SetupNameMapping()
        {
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(ICommandVerb).IsAssignableFrom(i.PluginType))
                .Select(i => i.ReturnedType ?? i.PluginType)
                .ToList();
            var nameMap = new Dictionary<string, CommandTypeAndDescription>();
            foreach (var commandType in commandTypes)
            {
                var attrs = commandType.GetCustomAttributes<CommandDetailsAttribute>().ToList();
                foreach (var attr in attrs)
                {
                    var name = attr.CommandName.ToLowerInvariant();
                    if (nameMap.ContainsKey(name))
                        continue;
                    nameMap.Add(name, new CommandTypeAndDescription(commandType, attr.Description ?? ""));
                }

                if (attrs.Count == 0)
                {
                    var name = commandType.Name.ToLowerInvariant();
                    if (name.EndsWith("verb"))
                        name = name.Substring(0, name.Length - 4);
                    if (name.EndsWith("command"))
                        name = name.Substring(0, name.Length - 7);
                    nameMap.Add(name, new CommandTypeAndDescription(commandType, ""));
                }
            }

            return nameMap;
        }

        public ICommandVerb GetCommandInstance(CompleteCommand command, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            var verb = command.Verb.ToLowerInvariant();
            var type = _nameMap.ContainsKey(verb) ? _nameMap[verb] : null;
            if (type == null)
                return null;
            var context = _container
                .With(environments)
                .With(state)
                .With(output)
                .With(command)
                .With(command.Arguments)
                .With(typeof(ICommandSource), this);
            if (environments.Current != null)
                context = context.With(environments.Current.GetType(), environments.Current);
            var commandObj = context.GetInstance(type.Type);
            return commandObj as ICommandVerb;
        }

        public IEnumerable<CommandDescription> GetAll()
        {
            return _nameMap.Select(kvp => new CommandDescription(kvp.Key, kvp.Value.Description ?? ""));
        }
    }
}