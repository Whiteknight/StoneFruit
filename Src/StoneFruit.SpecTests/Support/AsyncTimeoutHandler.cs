namespace StoneFruit.SpecTests.Support;

[Verb("async-timeout")]
public class AsyncTimeoutHandler : IAsyncHandler
{
    private readonly IOutput _output;

    public AsyncTimeoutHandler(IOutput output)
    {
        _output = output;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        _output.WriteLine("started");
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(100);
        }
        _output.WriteLine("stopped");
    }
}
