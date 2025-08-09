
namespace StoneFruit.SpecTests.Support;

public sealed class PredefinedInstanceHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
        => context.Output.WriteLine("Executed");
}

public sealed class PredefinedInstanceAsyncHandler : IAsyncHandler
{
    public Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
    {
        context.Output.WriteLine("Executed");
        return Task.CompletedTask;
    }
}
