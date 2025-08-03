namespace StoneFruit.SpecTests.Support;

[Verb("async-timeout")]
public class AsyncTimeoutHandler : IAsyncHandler
{
    public async Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken token)
    {
        context.Output.WriteLine("started");
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(100);
        }
        context.Output.WriteLine("stopped");
    }
}
