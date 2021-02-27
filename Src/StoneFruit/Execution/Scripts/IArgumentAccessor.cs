﻿using System.Collections.Generic;

namespace StoneFruit.Execution.Scripts
{
    public interface IArgumentAccessor
    {
        /// <summary>
        /// Given a set of arguments input to a script, return zero or more new arguments
        /// to pass to the scripted command
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerable<IArgument> Access(IArguments args);
    }
}