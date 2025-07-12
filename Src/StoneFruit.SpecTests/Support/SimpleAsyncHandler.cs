namespace StoneFruit.SpecTests.Support;

public class SimpleAsyncHandler : IAsyncHandler
{
    private readonly IOutput _output;

    public SimpleAsyncHandler(IOutput output)
    {
        _output = output;
    }

    public Task ExecuteAsync(CancellationToken cancellation)
    {
        _output.WriteLine("Simple async invoked");
        return Task.CompletedTask;
    }
}

