using System;
using StoneFruit.Execution;

namespace StoneFruit
{
    public delegate object TypeInstanceResolver(Type handlerType, IArguments command, CommandDispatcher dispatcher);
}