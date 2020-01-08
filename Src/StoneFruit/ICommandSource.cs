﻿using System;
using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    public interface ICommandSource
    {
        ICommandVerb GetCommandInstance(CompleteCommand command, IEnvironmentCollection environments, EngineState state, ITerminalOutput output);
        IEnumerable<Type> GetAll();
        Type GetCommandTypeByName(string name);
    }
}