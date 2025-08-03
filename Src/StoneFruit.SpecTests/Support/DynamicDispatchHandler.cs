using StoneFruit.Execution.Arguments;

namespace StoneFruit.SpecTests.Support;

[Verb("dispatch-target")]
public class DispatchTargetHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        context.Output.WriteLine("Invoked: " + arguments.Get(0).AsString());
    }
}

[Verb("dynamic-dispatch")]
public class DynamicDispatchHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        var type = arguments.Get("type").Require().MarkConsumed().AsString();
        if (type == "string")
            context.Dispatcher.Execute("dispatch-target test");
        if (type == "verbargs")
            context.Dispatcher.Execute("dispatch-target", arguments);
        if (type == "args")
        {
            var args = new PrependedVerbArguments(["dispatch-target"], arguments);
            context.Dispatcher.Execute(args);
        }
    }
}

[Verb("dynamic-dispatch-async")]
public class AsyncDynamicDispatchHandler : IAsyncHandler
{
    public async Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
    {
        var type = arguments.Get("type").Require().MarkConsumed().AsString();
        if (type == "string")
            await context.Dispatcher.ExecuteAsync("dispatch-target test");
        if (type == "verbargs")
            await context.Dispatcher.ExecuteAsync("dispatch-target", arguments);
        if (type == "args")
        {
            var args = new PrependedVerbArguments(["dispatch-target"], arguments);
            await context.Dispatcher.ExecuteAsync(args);
        }
    }
}