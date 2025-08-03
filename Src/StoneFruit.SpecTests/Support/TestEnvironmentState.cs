namespace StoneFruit.SpecTests.Support;

public class TestEnvironmentState
{
    public string Name { get; }

    public int IncrementCount { get; private set; }

    public TestEnvironmentState(string name)
    {
        Name = name;
        IncrementCount = 0;
    }

    public void Increment()
    {
        IncrementCount++;
    }
}

[Verb("increment-env-state-count")]
public class IncrementEnvStateCount : IHandler
{
    private readonly TestEnvironmentState _state;

    public IncrementEnvStateCount(TestEnvironmentState state)
    {
        _state = state;
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        _state.Increment();
        context.Output.WriteLine($"{_state.Name}: {_state.IncrementCount}");
    }
}
