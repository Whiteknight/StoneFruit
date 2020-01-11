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

        public ICommandVerb GetCommandInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            var verb = completeCommand.Verb.ToLowerInvariant();
            var type = _nameMap.ContainsKey(verb) ? _nameMap[verb] : null;
            return type == null ? null : ResolveCommand(completeCommand, dispatcher, type);
        }

        public ICommandVerb GetCommandInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
            where TCommand : class, ICommandVerb 
            => ResolveCommand(completeCommand, dispatcher, typeof(TCommand));

        public IReadOnlyDictionary<string, Type> GetAll() => _nameMap;

        public Type GetCommandTypeByName(string name)
            => _nameMap.ContainsKey(name) ? _nameMap[name] : null;

        private ICommandVerb ResolveCommand(CompleteCommand completeCommand, CommandDispatcher dispatcher, Type type)
        {
            var context = _container
                .With(dispatcher)
                .With(dispatcher.Environments)
                .With(dispatcher.State)
                .With(dispatcher.Output)
                .With(completeCommand)
                .With(completeCommand.Arguments)
                .With(typeof(ICommandSource), this);
            if (dispatcher.Environments.Current != null)
                context = context.With(dispatcher.Environments.Current.GetType(), dispatcher.Environments.Current);
            var commandObj = context.GetInstance(type);
            return commandObj as ICommandVerb;
        }
    }
}