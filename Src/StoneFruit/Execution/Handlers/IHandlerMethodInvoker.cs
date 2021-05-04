using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Invokes a handler method on an object, including mapping arguments where necessary.
    /// </summary>
    public interface IHandlerMethodInvoker
    {
        void Invoke(object instance, MethodInfo method, IArguments arguments, CommandDispatcher dispatcher, CancellationToken token);

        Task InvokeAsync(object instance, MethodInfo method, IArguments arguments, CommandDispatcher dispatcher, CancellationToken token);
    }
}
