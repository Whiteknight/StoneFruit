﻿using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Scripts.Formatting
{
    /// <summary>
    /// Return new FlagArguments for every unconsumed flag argument in the input
    /// </summary>
    public class FetchAllFlagsArgumentAccessor : IArgumentAccessor
    {
        public IEnumerable<IArgument> Access(IArguments args)
        {
            var results = new List<IArgument>();
            var flags = args.GetAllFlags();
            foreach (var flag in flags)
            {
                flag.MarkConsumed();
                results.Add(new FlagArgument(flag.Name));
            }

            return results;
        }
    }
}