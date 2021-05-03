using System;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// A method which takes a Type and arguments, and resolves an instance of that Type.
    /// </summary>
    /// <param name="handlerType"></param>
    /// <param name="command"></param>
    /// <param name="dispatcher"></param>
    /// <returns></returns>
    public delegate object? TypeInstanceResolver(Type handlerType, IArguments command, CommandDispatcher dispatcher);
}
