using StoneFruit;

namespace TestUtilities;

[Verb("test-environment")]
public class EnvironmentInjectionTestHandler : IHandler
{
    public TestEnvironment Environment { get; }

    public EnvironmentInjectionTestHandler(TestEnvironment environment)
    {
        Environment = environment;
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        context.Output.WriteLine(Environment.Value ?? "");
    }
}