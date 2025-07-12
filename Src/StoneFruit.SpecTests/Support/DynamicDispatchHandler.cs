using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.SpecTests.Support;

[Verb("dispatch-target")]
public class DispatchTargetHandler : IHandler
{
    private readonly IOutput _output;
    private readonly IArguments _arguments;

    public DispatchTargetHandler(IOutput output, IArguments arguments)
    {
        _output = output;
        _arguments = arguments;
    }

    public void Execute()
    {
        _output.WriteLine("Invoked: " + _arguments.Get(0).AsString());
    }
}

[Verb("dynamic-dispatch")]
public class DynamicDispatchHandler : IHandler
{
    private readonly IArguments _arguments;
    private readonly CommandDispatcher _dispatcher;

    public DynamicDispatchHandler(IArguments arguments, CommandDispatcher dispatcher)
    {
        _arguments = arguments;
        _dispatcher = dispatcher;
    }

    public void Execute()
    {
        var type = _arguments.Get("type").Require().MarkConsumed().AsString();
        if (type == "string")
            _dispatcher.Execute("dispatch-target test");
        if (type == "verbargs")
            _dispatcher.Execute("dispatch-target", _arguments);
        if (type == "args")
        {
            var args = new PrependedVerbArguments(["dispatch-target"], _arguments);
            _dispatcher.Execute(args);
        }
    }
}

[Verb("dynamic-dispatch-async")]
public class AsyncDynamicDispatchHandler : IAsyncHandler
{
    private readonly IArguments _arguments;
    private readonly CommandDispatcher _dispatcher;

    public AsyncDynamicDispatchHandler(IArguments arguments, CommandDispatcher dispatcher)
    {
        _arguments = arguments;
        _dispatcher = dispatcher;
    }

    public async Task ExecuteAsync(CancellationToken cancellation)
    {
        var type = _arguments.Get("type").Require().MarkConsumed().AsString();
        if (type == "string")
            await _dispatcher.ExecuteAsync("dispatch-target test");
        if (type == "verbargs")
            await _dispatcher.ExecuteAsync("dispatch-target", _arguments);
        if (type == "args")
        {
            var args = new PrependedVerbArguments(["dispatch-target"], _arguments);
            await _dispatcher.ExecuteAsync(args);
        }
    }
}