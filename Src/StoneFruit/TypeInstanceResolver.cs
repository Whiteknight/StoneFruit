using System;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit
{
    public delegate object TypeInstanceResolver(Type handlerType, IArguments command, CommandDispatcher dispatcher);
}