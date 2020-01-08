using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StoneFruit.Execution;
using StructureMap;

namespace StoneFruit.StructureMap
{
    public class StructureMapCommandSource : ICommandSource
    {
        private readonly IContainer _container;
        private readonly IReadOnlyDictionary<string, Type> _nameMap;

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

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(ICommandVerb).IsAssignableFrom(i.PluginType))
                .Select(i => i.ReturnedType ?? i.PluginType)
                .ToList();
            var nameMap = new Dictionary<string, Type>();
            foreach (var commandType in commandTypes)
            {
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
            var commandObj = context.GetInstance(type);
            return commandObj as ICommandVerb;
        }

        public IEnumerable<Type> GetAll() => _nameMap.Select(kvp => kvp.Value);

        public Type GetCommandTypeByName(string name)
            => _nameMap.ContainsKey(name) ? _nameMap[name] : null;
    }
}