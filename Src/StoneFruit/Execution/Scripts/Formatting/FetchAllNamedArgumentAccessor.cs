using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch all remaining unconsumed named arguments and return a new named argument
    /// for each with the same name and value
    /// </summary>
    public class FetchAllNamedArgumentAccessor : IArgumentAccessor
    {
        public IEnumerable<IArgument> Access(IArguments args)
        {
            var results = new List<IArgument>();
            foreach (var named in args.GetAllNamed())
            {
                named.MarkConsumed();
                results.Add(new NamedArgumentAccessor(named.Name, named.Value));
            }

            return results;
        }
    }
}