namespace StoneFruit.SpecTests.Support;

public class SimpleAsyncHandler : IAsyncHandler
{
    public Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
    {
        context.Output.WriteLine("Simple async invoked");
        return Task.CompletedTask;
    }
}

