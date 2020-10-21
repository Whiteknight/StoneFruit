using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class HandlerSourceCollection : IHandlers
    {
        //private readonly AliasMap _aliases;
        private readonly IReadOnlyList<IHandlerSource> _sources;

        public HandlerSourceCollection(IEnumerable<IHandlerSource> sources, AliasMap aliases)
        {
            //_aliases = aliases;
            _sources = sources.Where(s => s != null).ToList();
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            Assert.ArgumentNotNull(arguments, nameof(arguments));
            //command = _aliases.Translate(command);
            foreach (var source in _sources)
            {
                var handler = source.GetInstance(arguments, dispatcher);
                if (handler != null)
                    return handler;
            }
            return null;
            //.Select(source => source.GetInstance(arguments, dispatcher))
            //.FirstOrDefault(handler => handler != null);
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            var allVerbs = _sources.SelectMany(s => s.GetAll())
                .GroupBy(info => info.Verb)
                .Select(g => g.First())
                .ToDictionary(v => v.Verb);
            //foreach (var aliasKvp in _aliases)
            //{
            //    var verb = aliasKvp.Value;
            //    var alias = aliasKvp.Key;
            //    if (!allVerbs.ContainsKey(alias) && allVerbs.ContainsKey(verb))
            //    {
            //        var verbInfo = allVerbs[verb];
            //        var info = _aliases.GetAliasInfoFromVerbInfo(alias, verb, verbInfo);
            //        allVerbs.Add(alias, info);
            //    }
            //}
            return allVerbs.Values;
        }

        public IVerbInfo GetByName(Verb verb)
        {
            var byName = _sources
                .Select(source => source.GetByName(verb))
                .FirstOrDefault(info => info != null);
            if (byName != null)
                return byName;

            //var aliased = _aliases.GetVerb(name);
            //var byAlias = _sources
            //    .Select(source => source.GetByName(aliased))
            //    .FirstOrDefault(info => info != null);

            //if (byAlias != null)
            //    return _aliases.GetAliasInfoFromVerbInfo(name, aliased, byAlias);

            return null;
        }
    }
}
