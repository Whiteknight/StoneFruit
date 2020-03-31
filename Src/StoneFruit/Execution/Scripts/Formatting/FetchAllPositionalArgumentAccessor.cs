﻿using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Fetch all remaining unconsumed positional arguments and return a new positional
    /// for each with the same value
    /// </summary>
    public class FetchAllPositionalArgumentAccessor : IArgumentAccessor
    {
        public IEnumerable<IArgument> Access(ICommandArguments args)
        {
            var results = new List<IArgument>();
            foreach (var positional in args.GetAllPositionals())
            {
                positional.MarkConsumed();
                results.Add(new PositionalArgumentAccessor(positional.Value));
            }
            return results;
        }
    }
}